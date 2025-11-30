namespace FreeLink.Application.UseCase.ActivityLogs.DTOs;

public class ActivityLogDto
{
    public int ActivityId { get; set; }
    public int ProjectId { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string? ActivityDescription { get; set; }
    public DateTime CreatedAt { get; set; }
}
