using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetTransactionStatistics;

public class GetTransactionStatisticsHandler : IRequestHandler<GetTransactionStatisticsQuery, GetTransactionStatisticsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTransactionStatisticsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetTransactionStatisticsResponse> Handle(GetTransactionStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetTransactionStatisticsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver estadísticas de transacciones"
                };
            }

            // Obtener todas las transacciones
            var allTransactions = await _unitOfWork.Repository<Transaction>().GetAll();
            var transactions = allTransactions.AsEnumerable();

            // Aplicar filtros de fecha
            if (request.StartDate.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                transactions = transactions.Where(t => t.CreatedAt <= request.EndDate.Value);
            }

            var transactionsList = transactions.ToList();

            // Calcular estadísticas
            var statistics = new TransactionStatisticsDto
            {
                TotalTransactions = transactionsList.Count,
                TotalAmount = transactionsList.Sum(t => t.Amount),
                AverageTransactionAmount = transactionsList.Any() ? transactionsList.Average(t => t.Amount) : 0,
                SuccessfulTransactions = transactionsList.Count(t => t.TransactionStatus == "Completada"),
                PendingTransactions = transactionsList.Count(t => t.TransactionStatus == "Pendiente"),
                FailedTransactions = transactionsList.Count(t => t.TransactionStatus == "Fallida"),
                TransactionsByType = transactionsList
                    .GroupBy(t => t.TransactionType ?? "Sin Tipo")
                    .ToDictionary(g => g.Key, g => g.Count()),
                AmountByType = transactionsList
                    .GroupBy(t => t.TransactionType ?? "Sin Tipo")
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount))
            };

            return new GetTransactionStatisticsResponse
            {
                Success = true,
                Message = "Estadísticas obtenidas exitosamente",
                Data = statistics
            };
        }
        catch (Exception ex)
        {
            return new GetTransactionStatisticsResponse
            {
                Success = false,
                Message = $"Error al obtener estadísticas: {ex.Message}"
            };
        }
    }
}
