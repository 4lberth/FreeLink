using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MarkNotificationAsReadResponse> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var notification = await _unitOfWork.Repository<Notification>().GetById(request.NotificationId);
            
            if (notification == null)
            {
                return new MarkNotificationAsReadResponse
                {
                    Success = false,
                    Message = "Notificación no encontrada"
                };
            }

            // Verificar que la notificación pertenece al usuario
            if (notification.UserId != request.UserId)
            {
                return new MarkNotificationAsReadResponse
                {
                    Success = false,
                    Message = "No tienes permiso para marcar esta notificación"
                };
            }

            // Marcar como leída
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.Complete();

            return new MarkNotificationAsReadResponse
            {
                Success = true,
                Message = "Notificación marcada como leída"
            };
        }
        catch (Exception ex)
        {
            return new MarkNotificationAsReadResponse
            {
                Success = false,
                Message = $"Error al marcar notificación: {ex.Message}"
            };
        }
    }
}
