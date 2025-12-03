using MediatR;

namespace FreeLink.Application.UseCase.Reports.Commands.CreateReport;

public class CreateReportCommand : IRequest<CreateReportResponse>
{
    public int ReporterId { get; set; }
    public int? ReportedUserId { get; set; }
    public int? ReportedProjectId { get; set; }
    public int? ReportedMessageId { get; set; }
    public string ReportReason { get; set; } = string.Empty;
    public string? ReportDescription { get; set; }
}
