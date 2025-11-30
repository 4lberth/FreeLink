using FreeLink.Application.UseCase.Notifications.DTOs;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Notifications.Queries.GetUserNotifications;

public class GetUserNotificationsHandler : IRequestHandler<GetUserNotificationsQuery, GetUserNotificationsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserNotificationsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUserNotificationsResponse> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener notificaciones del usuario
            var allNotifications = await _unitOfWork.Repository<Notification>()
                .GetAsync(n => n.UserId == request.UserId);

            var notificationsList = allNotifications.OrderByDescending(n => n.CreatedAt).ToList();

            // Filtrar por no leídas si se especifica
            if (request.UnreadOnly)
            {
                notificationsList = notificationsList.Where(n => n.IsRead == false).ToList();
            }

            var notificationDtos = notificationsList.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                NotificationType = n.NotificationType,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead ?? false,
                ReadAt = n.ReadAt,
                RelatedResourceType = n.RelatedResourceType,
                RelatedResourceId = n.RelatedResourceId,
                CreatedAt = n.CreatedAt
            }).ToList();

            var unreadCount = notificationsList.Count(n => n.IsRead == false);

            return new GetUserNotificationsResponse
            {
                Success = true,
                Message = $"{notificationDtos.Count} notificaciones encontradas",
                Notifications = notificationDtos,
                TotalCount = notificationDtos.Count,
                UnreadCount = unreadCount
            };
        }
        catch (Exception ex)
        {
            return new GetUserNotificationsResponse
            {
                Success = false,
                Message = $"Error al obtener notificaciones: {ex.Message}"
            };
        }
    }
}
