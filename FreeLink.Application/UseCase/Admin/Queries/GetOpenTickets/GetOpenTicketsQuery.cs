using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetOpenTickets;

public class GetOpenTicketsQuery : IRequest<GetOpenTicketsResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; } // Filter by status
    public string? Priority { get; set; } // Filter by priority
}
