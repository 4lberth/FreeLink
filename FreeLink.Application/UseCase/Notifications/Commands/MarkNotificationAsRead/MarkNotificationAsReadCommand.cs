using MediatR;

namespace FreeLink.Application.UseCase.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommand : IRequest<MarkNotificationAsReadResponse>
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
}
