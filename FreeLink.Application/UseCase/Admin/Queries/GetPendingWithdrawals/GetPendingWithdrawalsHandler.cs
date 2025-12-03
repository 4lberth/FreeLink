using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingWithdrawals;

public class GetPendingWithdrawalsHandler : IRequestHandler<GetPendingWithdrawalsQuery, GetPendingWithdrawalsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingWithdrawalsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetPendingWithdrawalsResponse> Handle(GetPendingWithdrawalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetPendingWithdrawalsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver retiros pendientes"
                };
            }

            // Obtener todas las transacciones de tipo "Retiro" con estado "Pendiente"
            var allTransactions = await _unitOfWork.Repository<Transaction>().GetAll();
            var withdrawals = allTransactions
                .Where(t => t.TransactionType == "Retiro" && t.TransactionStatus == "Pendiente")
                .OrderByDescending(t => t.CreatedAt)
                .AsEnumerable();

            var totalCount = withdrawals.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedWithdrawals = withdrawals
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener información de usuarios
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear a DTOs
            var withdrawalDtos = pagedWithdrawals.Select(w =>
            {
                var user = usersList.FirstOrDefault(u => u.UserId == w.FromUserId);
                return new WithdrawalDto
                {
                    TransactionId = w.TransactionId,
                    UserId = w.FromUserId ?? 0,
                    UserName = user?.Email ?? "Usuario desconocido",
                    UserEmail = user?.Email ?? "N/A",
                    Amount = w.Amount,
                    Status = w.TransactionStatus ?? "Pendiente",
                    RequestDate = w.CreatedAt,
                    Description = w.Description ?? string.Empty
                };
            }).ToList();

            return new GetPendingWithdrawalsResponse
            {
                Success = true,
                Message = $"Se encontraron {totalCount} retiros pendientes",
                Withdrawals = withdrawalDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetPendingWithdrawalsResponse
            {
                Success = false,
                Message = $"Error al obtener retiros pendientes: {ex.Message}"
            };
        }
    }
}
