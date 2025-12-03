using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllTransactions;

public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsQuery, GetAllTransactionsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTransactionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllTransactionsResponse> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetAllTransactionsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver transacciones"
                };
            }

            // Obtener todas las transacciones
            var allTransactions = await _unitOfWork.Repository<Transaction>().GetAll();
            var transactions = allTransactions.AsEnumerable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.TransactionType))
            {
                transactions = transactions.Where(t => t.TransactionType == request.TransactionType);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                transactions = transactions.Where(t => t.TransactionStatus == request.Status);
            }

            if (request.StartDate.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= request.EndDate.Value);
            }

            transactions = transactions.OrderByDescending(t => t.CreatedAt);

            var totalCount = transactions.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedTransactions = transactions
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener usuarios
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Note: Transaction doesn't have a direct ProjectId link
            // Projects are linked through Escrow accounts which have a collection of Transactions
            // For now, we'll leave ProjectId as null since we can't easily reverse the relationship

            // Mapear a DTOs (FromUserId and ToUserId are nullable)
            var transactionDtos = pagedTransactions.Select(t =>
            {
                var fromUser = t.FromUserId.HasValue ? usersList.FirstOrDefault(u => u.UserId == t.FromUserId.Value) : null;
                var toUser = t.ToUserId.HasValue ? usersList.FirstOrDefault(u => u.UserId == t.ToUserId.Value) : null;

                return new TransactionListDto
                {
                    TransactionId = t.TransactionId,
                    FromUserId = t.FromUserId ?? 0,
                    FromUserName = fromUser?.Email ?? "Usuario no encontrado",
                    ToUserId = t.ToUserId ?? 0,
                    ToUserName = toUser?.Email ?? "Usuario no encontrado",
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    Status = t.TransactionStatus,
                    CreatedAt = t.CreatedAt,
                    ProjectId = null,
                    ProjectTitle = null
                };
            }).ToList();

            return new GetAllTransactionsResponse
            {
                Success = true,
                Message = "Transacciones obtenidas exitosamente",
                Transactions = transactionDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetAllTransactionsResponse
            {
                Success = false,
                Message = $"Error al obtener transacciones: {ex.Message}"
            };
        }
    }
}
