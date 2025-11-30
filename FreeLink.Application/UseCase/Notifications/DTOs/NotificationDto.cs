namespace FreeLink.Application.UseCase.Notifications.DTOs;

public class NotificationDto
{
    public int NotificationId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? RelatedResourceType { get; set; }
    public int? RelatedResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
