using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetTransactionDetails;

public class GetTransactionDetailsHandler : IRequestHandler<GetTransactionDetailsQuery, GetTransactionDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTransactionDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetTransactionDetailsResponse> Handle(GetTransactionDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetTransactionDetailsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver detalles de transacciones"
                };
            }

            // Obtener la transacción
            var transaction = await _unitOfWork.Repository<Transaction>().GetById(request.TransactionId);
            if (transaction == null)
            {
                return new GetTransactionDetailsResponse
                {
                    Success = false,
                    Message = "Transacción no encontrada"
                };
            }

            // Obtener usuarios relacionados (FromUserId and ToUserId are nullable)
            FreeLink.Domain.Entities.User? fromUser = null;
            FreeLink.Domain.Entities.User? toUser = null;

            if (transaction.FromUserId.HasValue)
            {
                fromUser = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(transaction.FromUserId.Value);
            }

            if (transaction.ToUserId.HasValue)
            {
                toUser = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(transaction.ToUserId.Value);
            }

            // Note: Transaction doesn't have a direct ProjectId link
            // Projects are linked through Escrow accounts which have a collection of Transactions
            // For now, we'll leave ProjectId as null since we can't easily reverse the relationship

            var transactionDetails = new TransactionDetailsDto
            {
                TransactionId = transaction.TransactionId,
                FromUserId = transaction.FromUserId ?? 0,
                FromUserName = fromUser?.Email ?? "Usuario no encontrado",
                FromUserEmail = fromUser?.Email ?? "",
                ToUserId = transaction.ToUserId ?? 0,
                ToUserName = toUser?.Email ?? "Usuario no encontrado",
                ToUserEmail = toUser?.Email ?? "",
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                Status = transaction.TransactionStatus,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt,
                ProjectId = null,
                ProjectTitle = null,
                PaymentMethod = null,
                TransactionReference = null
            };

            return new GetTransactionDetailsResponse
            {
                Success = true,
                Message = "Detalles de transacción obtenidos exitosamente",
                Data = transactionDetails
            };
        }
        catch (Exception ex)
        {
            return new GetTransactionDetailsResponse
            {
                Success = false,
                Message = $"Error al obtener detalles de transacción: {ex.Message}"
            };
        }
    }
}
