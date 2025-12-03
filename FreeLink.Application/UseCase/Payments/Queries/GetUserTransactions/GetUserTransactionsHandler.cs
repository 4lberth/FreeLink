using FreeLink.Application.UseCase.Payments.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Payments.Queries.GetUserTransactions;

public class GetUserTransactionsHandler : IRequestHandler<GetUserTransactionsQuery, GetUserTransactionsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserTransactionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserTransactionsResponse> Handle(GetUserTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener transacciones del usuario (como emisor o receptor)
            var transactionsQuery = await _unitOfWork.Repository<Transaction>()
                .GetAsync(t => t.FromUserId == request.UserId || t.ToUserId == request.UserId);

            var transactions = transactionsQuery.ToList();

            // 2. Aplicar filtros
            if (!string.IsNullOrWhiteSpace(request.TransactionType))
            {
                transactions = transactions
                    .Where(t => t.TransactionType == request.TransactionType)
                    .ToList();
            }

            if (request.StartDate.HasValue)
            {
                transactions = transactions
                    .Where(t => t.CreatedAt >= request.StartDate.Value)
                    .ToList();
            }

            if (request.EndDate.HasValue)
            {
                transactions = transactions
                    .Where(t => t.CreatedAt <= request.EndDate.Value)
                    .ToList();
            }

            if (request.ProjectId.HasValue && request.ProjectId.Value > 0)
            {
                // Obtener escrows del proyecto
                var escrowsQuery = await _unitOfWork.Repository<Escrowaccount>()
                    .GetAsync(e => e.ProjectId == request.ProjectId.Value);
                var escrowIds = escrowsQuery.Select(e => e.EscrowId).ToList();

                transactions = transactions
                    .Where(t => t.EscrowId.HasValue && escrowIds.Contains(t.EscrowId.Value))
                    .ToList();
            }

            // 3. Ordenar por fecha (más recientes primero) y limitar
            transactions = transactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(request.Limit)
                .ToList();

            // 4. Obtener información de usuarios
            var userIds = transactions
                .SelectMany(t => new[] { t.FromUserId, t.ToUserId })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var usersQuery = await _unitOfWork.Repository<Domain.Entities.User>()
                .GetAsync(u => userIds.Contains(u.UserId));
            var users = usersQuery.ToList();

            // 5. Obtener información de proyectos (via escrow)
            var escrowIdsInTransactions = transactions
                .Where(t => t.EscrowId.HasValue)
                .Select(t => t.EscrowId!.Value)
                .Distinct()
                .ToList();

            var escrowsForTransactions = new List<Escrowaccount>();
            var projects = new List<Project>();

            if (escrowIdsInTransactions.Any())
            {
                var escrowsQueryForTrans = await _unitOfWork.Repository<Escrowaccount>()
                    .GetAsync(e => escrowIdsInTransactions.Contains(e.EscrowId));
                escrowsForTransactions = escrowsQueryForTrans.ToList();

                var projectIds = escrowsForTransactions.Select(e => e.ProjectId).Distinct().ToList();
                var projectsQuery = await _unitOfWork.Repository<Project>()
                    .GetAsync(p => projectIds.Contains(p.ProjectId));
                projects = projectsQuery.ToList();
            }

            // 6. Mapear a DTOs
            var transactionDtos = transactions.Select(t =>
            {
                var fromUser = t.FromUserId.HasValue
                    ? users.FirstOrDefault(u => u.UserId == t.FromUserId.Value)
                    : null;

                var toUser = t.ToUserId.HasValue
                    ? users.FirstOrDefault(u => u.UserId == t.ToUserId.Value)
                    : null;

                var escrow = t.EscrowId.HasValue
                    ? escrowsForTransactions.FirstOrDefault(e => e.EscrowId == t.EscrowId.Value)
                    : null;

                var project = escrow != null
                    ? projects.FirstOrDefault(p => p.ProjectId == escrow.ProjectId)
                    : null;

                return new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    EscrowId = t.EscrowId,
                    FromUserId = t.FromUserId,
                    FromUserName = fromUser?.Email,
                    ToUserId = t.ToUserId,
                    ToUserName = toUser?.Email ?? (t.TransactionType == "Commission" ? "Plataforma" : null),
                    Amount = t.Amount,
                    TransactionType = t.TransactionType,
                    TransactionStatus = t.TransactionStatus ?? "Pending",
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    ReceiptUrl = t.ReceiptUrl,
                    ProjectTitle = project?.Title
                };
            }).ToList();

            // 7. Calcular balance
            decimal balance = CalculateBalance(request.UserId, transactions);

            return new GetUserTransactionsResponse
            {
                Success = true,
                Message = "Transacciones obtenidas exitosamente",
                Balance = balance,
                Transactions = transactionDtos
            };
        }
        catch (Exception ex)
        {
            return new GetUserTransactionsResponse
            {
                Success = false,
                Message = $"Error al obtener transacciones: {ex.Message}"
            };
        }
    }

    private decimal CalculateBalance(int userId, List<Transaction> allUserTransactions)
    {
        decimal balance = 0;

        foreach (var transaction in allUserTransactions.Where(t => t.TransactionStatus == "Completed"))
        {
            // Dinero recibido
            if (transaction.ToUserId == userId && transaction.TransactionType == "Release")
            {
                balance += transaction.Amount;
            }

            // Dinero enviado/gastado
            if (transaction.FromUserId == userId && transaction.TransactionType == "Deposit")
            {
                balance -= transaction.Amount;
            }

            // Retiros (cuando se implementen)
            if (transaction.FromUserId == userId && transaction.TransactionType == "Withdrawal")
            {
                balance -= transaction.Amount;
            }
        }

        return balance;
    }
}
