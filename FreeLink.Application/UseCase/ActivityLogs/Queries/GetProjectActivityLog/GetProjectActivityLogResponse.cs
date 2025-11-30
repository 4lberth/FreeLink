using FreeLink.Application.UseCase.ActivityLogs.DTOs;

namespace FreeLink.Application.UseCase.ActivityLogs.Queries.GetProjectActivityLog;

public class GetProjectActivityLogResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ActivityLogDto> Activities { get; set; } = new();
}
