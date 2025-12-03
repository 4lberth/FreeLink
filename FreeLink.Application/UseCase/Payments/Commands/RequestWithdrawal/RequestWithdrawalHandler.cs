using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.RequestWithdrawal;

public class RequestWithdrawalHandler : IRequestHandler<RequestWithdrawalCommand, RequestWithdrawalResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IWalletService _walletService;

    public RequestWithdrawalHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IWalletService walletService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _walletService = walletService;
    }

    public async Task<RequestWithdrawalResponse> Handle(RequestWithdrawalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar monto
            if (request.Amount <= 0)
            {
                return new RequestWithdrawalResponse
                {
                    Success = false,
                    Message = "El monto debe ser mayor a cero"
                };
            }

            // 2. Calcular balance disponible del usuario
            var transactionsQuery = await _unitOfWork.Repository<Transaction>()
                .GetAsync(t => (t.FromUserId == request.UserId || t.ToUserId == request.UserId) 
                           && t.TransactionStatus == "Completada");
            var transactions = transactionsQuery.ToList();

            decimal availableBalance = 0;

            foreach (var transaction in transactions)
            {
                // Dinero recibido
                if (transaction.ToUserId == request.UserId && transaction.TransactionType == "Liberación")
                {
                    availableBalance += transaction.Amount;
                }

                // Dinero gastado
                if (transaction.FromUserId == request.UserId && transaction.TransactionType == "Depósito")
                {
                    availableBalance -= transaction.Amount;
                }

                // Retiros previos
                if (transaction.FromUserId == request.UserId && transaction.TransactionType == "Retiro")
                {
                    availableBalance -= transaction.Amount;
                }
            }

            // 3. Verificar que hay balance suficiente
            if (availableBalance < request.Amount)
            {
                return new RequestWithdrawalResponse
                {
                    Success = false,
                    Message = $"Balance insuficiente. Disponible: ${availableBalance:F2}"
                };
            }

            // 4. Crear transacción de retiro en estado Pending
            var withdrawalTransaction = new Transaction
            {
                FromUserId = request.UserId,
                ToUserId = null, // El retiro sale del sistema
                Amount = request.Amount,
                TransactionType = "Retiro",
                TransactionStatus = "Pendiente", // Requiere aprobación manual
                Description = "Solicitud de retiro de fondos",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Transaction>().Add(withdrawalTransaction);
            await _unitOfWork.Complete();

            // NOTA: El wallet se actualizará cuando un admin apruebe el retiro (status="Completada")
            // Por ahora el retiro queda pendiente y no afecta el balance

            // 5. Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                userId: request.UserId,
                type: "WithdrawalRequested",
                title: "Solicitud de retiro enviada",
                message: $"Tu solicitud de retiro de ${request.Amount:F2} está siendo procesada",
                resourceType: "Transaction",
                resourceId: withdrawalTransaction.TransactionId
            );

            // NOTA: En un sistema real, aquí también se notificaría a administradores
            // para que procesen el retiro manualmente

            return new RequestWithdrawalResponse
            {
                Success = true,
                Message = "Solicitud de retiro creada exitosamente. Será procesada por el equipo",
                WithdrawalId = withdrawalTransaction.TransactionId,
                Amount = request.Amount,
                Status = "Pending"
            };
        }
        catch (Exception ex)
        {
            return new RequestWithdrawalResponse
            {
                Success = false,
                Message = $"Error al solicitar retiro: {ex.Message}"
            };
        }
    }
}
