using MediatR;

namespace FreeLink.Application.UseCase.Payments.Queries.GetUserTransactions;

public class GetUserTransactionsQuery : IRequest<GetUserTransactionsResponse>
{
    public int UserId { get; set; }
    public string? TransactionType { get; set; } // null = todos
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ProjectId { get; set; }
    public int Limit { get; set; } = 50;
}
