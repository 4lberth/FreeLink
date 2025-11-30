using MediatR;

namespace FreeLink.Application.UseCase.Notifications.Queries.GetUserNotifications;

public class GetUserNotificationsQuery : IRequest<GetUserNotificationsResponse>
{
    public int UserId { get; set; }
    public bool UnreadOnly { get; set; } = false;
}
