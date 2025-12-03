using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.UpdateTicketStatus;

public class UpdateTicketStatusHandler : IRequestHandler<UpdateTicketStatusCommand, UpdateTicketStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public UpdateTicketStatusHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<UpdateTicketStatusResponse> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new UpdateTicketStatusResponse
                {
                    Success = false,
                    Message = "No tienes permisos para actualizar tickets"
                };
            }

            // Obtener el ticket
            var ticket = await _unitOfWork.Repository<Supportticket>().GetById(request.TicketId);
            if (ticket == null)
            {
                return new UpdateTicketStatusResponse
                {
                    Success = false,
                    Message = "Ticket no encontrado"
                };
            }

            // Validar estado
            var validStatuses = new[] { "Abierto", "En Progreso", "Resuelto", "Cerrado" };
            if (!validStatuses.Contains(request.NewStatus))
            {
                return new UpdateTicketStatusResponse
                {
                    Success = false,
                    Message = "Estado inválido. Debe ser: Abierto, En Progreso, Resuelto o Cerrado"
                };
            }

            var oldStatus = ticket.TicketStatus;
            ticket.TicketStatus = request.NewStatus;

            // Si se marca como resuelto o cerrado, establecer ResolvedAt
            if ((request.NewStatus == "Resuelto" || request.NewStatus == "Cerrado") && ticket.ResolvedAt == null)
            {
                ticket.ResolvedAt = DateTime.UtcNow;
            }

            await _unitOfWork.Repository<Supportticket>().Update(ticket);

            // Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                ticket.UserId,
                "admin_action",
                "Estado de Ticket Actualizado",
                $"Tu ticket #{ticket.TicketId} cambió de '{oldStatus}' a '{request.NewStatus}'",
                "ticket",
                ticket.TicketId
            );

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "UpdateTicketStatus",
                $"Actualizó ticket #{request.TicketId} a estado '{request.NewStatus}'"
            );

            await _unitOfWork.Complete();

            return new UpdateTicketStatusResponse
            {
                Success = true,
                Message = "Estado del ticket actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new UpdateTicketStatusResponse
            {
                Success = false,
                Message = $"Error al actualizar ticket: {ex.Message}"
            };
        }
    }
}
