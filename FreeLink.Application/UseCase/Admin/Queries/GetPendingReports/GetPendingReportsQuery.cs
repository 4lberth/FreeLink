using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingReports;

public class GetPendingReportsQuery : IRequest<GetPendingReportsResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportType { get; set; } // Filter by report type
}
