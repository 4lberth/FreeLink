using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.DismissReport;

public class DismissReportCommand : IRequest<DismissReportResponse>
{
    public int ReportId { get; set; }
    public int RequestingAdminId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
