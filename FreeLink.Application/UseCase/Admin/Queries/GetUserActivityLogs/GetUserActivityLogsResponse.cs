namespace FreeLink.Application.UseCase.Admin.Queries.GetUserActivityLogs;

public class GetUserActivityLogsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<UserActivityLogDto> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class UserActivityLogDto
{
    public int LogId { get; set; }
    public string? ActivityType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? RelatedEntity { get; set; }
    public int? RelatedEntityId { get; set; }
}
