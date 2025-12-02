using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;

namespace FreeLink.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogProjectActivityAsync(int projectId, int? userId, string activityType, string description)
    {
        var log = new Projectactivitylog
        {
            ProjectId = projectId,
            UserId = userId,
            ActivityType = activityType,
            ActivityDescription = description,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Projectactivitylog>().Add(log);
        await _unitOfWork.Complete();
    }

    public async Task CreateNotificationAsync(int userId, string type, string title, string message, 
        string? resourceType = null, int? resourceId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            NotificationType = type,
            Title = title,
            Message = message,
            IsRead = false,
            RelatedResourceType = resourceType,
            RelatedResourceId = resourceId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Notification>().Add(notification);
        await _unitOfWork.Complete();
    }

    public async Task CreateMultipleNotificationsAsync(List<int> userIds, string type, string title, string message,
        string? resourceType = null, int? resourceId = null)
    {
        foreach (var userId in userIds)
        {
            await CreateNotificationAsync(userId, type, title, message, resourceType, resourceId);
        }
    }
}