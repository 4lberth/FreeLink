namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingReports;

public class GetPendingReportsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ReportListDto> Reports { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class ReportListDto
{
    public int ReportId { get; set; }
    public int ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public int ReportedUserId { get; set; }
    public string ReportedUserName { get; set; } = string.Empty;
    public string? ReportType { get; set; }
    public string? ReportReason { get; set; }
    public string? ReportStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
}
