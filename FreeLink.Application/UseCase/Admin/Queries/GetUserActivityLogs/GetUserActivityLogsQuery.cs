using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetUserActivityLogs;

public class GetUserActivityLogsQuery : IRequest<GetUserActivityLogsResponse>
{
    public int UserId { get; set; }
    public int RequestingAdminId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
