using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllTransactions;

public class GetAllTransactionsQuery : IRequest<GetAllTransactionsResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TransactionType { get; set; } // Filter by type
    public string? Status { get; set; } // Filter by status
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
