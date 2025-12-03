using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingDisputes;

public class GetPendingDisputesQuery : IRequest<GetPendingDisputesResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; } // Filter by status
}
