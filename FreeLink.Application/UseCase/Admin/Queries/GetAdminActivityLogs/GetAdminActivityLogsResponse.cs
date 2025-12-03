namespace FreeLink.Application.UseCase.Admin.Queries.GetAdminActivityLogs;

public class GetAdminActivityLogsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<AdminActivityLogDto> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class AdminActivityLogDto
{
    public int LogId { get; set; }
    public int AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public string? ActionType { get; set; }
    public string? ActionDetails { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? IpAddress { get; set; }
}
