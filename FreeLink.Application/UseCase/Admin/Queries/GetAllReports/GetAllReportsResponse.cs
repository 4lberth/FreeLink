namespace FreeLink.Application.UseCase.Admin.Queries.GetAllReports;

public class GetAllReportsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ReportDto> Reports { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class ReportDto
{
    public int ReportId { get; set; }
    public int ReporterId { get; set; }
    public string ReporterEmail { get; set; } = string.Empty;
    public int? ReportedUserId { get; set; }
    public string? ReportedUserEmail { get; set; }
    public int? ReportedProjectId { get; set; }
    public string? ReportedProjectTitle { get; set; }
    public int? ReportedMessageId { get; set; }
    public string ReportReason { get; set; } = string.Empty;
    public string? ReportDescription { get; set; }
    public string ReportStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewedByEmail { get; set; }
    public string? Resolution { get; set; }
    public string ReportType { get; set; } = string.Empty; // "Usuario", "Proyecto", "Mensaje"
}
