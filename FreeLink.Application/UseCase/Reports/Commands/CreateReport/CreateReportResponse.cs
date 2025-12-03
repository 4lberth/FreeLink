namespace FreeLink.Application.UseCase.Reports.Commands.CreateReport;

public class CreateReportResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ReportId { get; set; }
}
