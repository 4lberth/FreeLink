using FreeLink.Application.UseCase.Notifications.DTOs;

namespace FreeLink.Application.UseCase.Notifications.Queries.GetUserNotifications;

public class GetUserNotificationsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<NotificationDto> Notifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
}
