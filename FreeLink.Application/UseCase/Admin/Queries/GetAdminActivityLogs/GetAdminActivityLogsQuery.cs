using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAdminActivityLogs;

public class GetAdminActivityLogsQuery : IRequest<GetAdminActivityLogsResponse>
{
    public int RequestingAdminId { get; set; }
    public int? AdminId { get; set; } // Filter by specific admin
    public string? ActionType { get; set; } // Filter by action type
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
