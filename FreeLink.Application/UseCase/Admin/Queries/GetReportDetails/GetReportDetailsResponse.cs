namespace FreeLink.Application.UseCase.Admin.Queries.GetReportDetails;

public class GetReportDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ReportDetailsDto? Data { get; set; }
}

public class ReportDetailsDto
{
    public int ReportId { get; set; }
    public int ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string ReporterEmail { get; set; } = string.Empty;
    public int ReportedUserId { get; set; }
    public string ReportedUserName { get; set; } = string.Empty;
    public string ReportedUserEmail { get; set; } = string.Empty;
    public string? ReportType { get; set; }
    public string? ReportReason { get; set; }
    public string? Description { get; set; }
    public string? EvidenceUrl { get; set; }
    public string? ReportStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
    public string? Resolution { get; set; }
    public string? AdminAction { get; set; }
    public int? ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
}
