using FreeLink.Application.UseCase.Payments.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Payments.Queries.GetUserBalance;

public class GetUserBalanceHandler : IRequestHandler<GetUserBalanceQuery, GetUserBalanceResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserBalanceHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserBalanceResponse> Handle(GetUserBalanceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener todas las transacciones del usuario
            var transactionsQuery = await _unitOfWork.Repository<Transaction>()
                .GetAsync(t => t.FromUserId == request.UserId || t.ToUserId == request.UserId);
            var transactions = transactionsQuery.ToList();

            // 2. Calcular balance disponible (transacciones completadas)
            decimal availableBalance = 0;
            decimal totalEarnings = 0;
            decimal totalWithdrawals = 0;
            decimal totalSpent = 0;

            var completedTransactions = transactions.Where(t => t.TransactionStatus == "Completada").ToList();

            foreach (var transaction in completedTransactions)
            {
                // Dinero recibido (Liberación)
                if (transaction.ToUserId == request.UserId && transaction.TransactionType == "Liberación")
                {
                    availableBalance += transaction.Amount;
                    totalEarnings += transaction.Amount;
                }

                // Dinero gastado (Depósito)
                if (transaction.FromUserId == request.UserId && transaction.TransactionType == "Depósito")
                {
                    availableBalance -= transaction.Amount;
                    totalSpent += transaction.Amount;
                }

                // Retiros realizados
                if (transaction.FromUserId == request.UserId && transaction.TransactionType == "Retiro" && transaction.TransactionStatus == "Completada")
                {
                    availableBalance -= transaction.Amount;
                    totalWithdrawals += transaction.Amount;
                }
            }

            // 3. Calcular balance pendiente (transacciones pendientes)
            decimal pendingBalance = 0;
            var pendingTransactions = transactions.Where(t => t.TransactionStatus == "Pendiente").ToList();

            foreach (var transaction in pendingTransactions)
            {
                if (transaction.ToUserId == request.UserId)
                {
                    pendingBalance += transaction.Amount;
                }
            }

            // 4. Contar transacciones
            int completedCount = completedTransactions.Count;
            int pendingCount = pendingTransactions.Count;

            // 5. Crear DTO
            var balanceDto = new BalanceDto
            {
                UserId = request.UserId,
                AvailableBalance = availableBalance,
                PendingBalance = pendingBalance,
                TotalEarnings = totalEarnings,
                TotalWithdrawals = totalWithdrawals,
                TotalSpent = totalSpent,
                CompletedTransactions = completedCount,
                PendingTransactions = pendingCount
            };

            return new GetUserBalanceResponse
            {
                Success = true,
                Message = "Balance obtenido exitosamente",
                Balance = balanceDto
            };
        }
        catch (Exception ex)
        {
            return new GetUserBalanceResponse
            {
                Success = false,
                Message = $"Error al obtener balance: {ex.Message}"
            };
        }
    }
}
