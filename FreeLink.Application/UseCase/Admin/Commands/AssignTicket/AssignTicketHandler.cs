using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.AssignTicket;

public class AssignTicketHandler : IRequestHandler<AssignTicketCommand, AssignTicketResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public AssignTicketHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<AssignTicketResponse> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var requestingAdmin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (requestingAdmin == null || requestingAdmin.UserType != "Administrador")
            {
                return new AssignTicketResponse
                {
                    Success = false,
                    Message = "No tienes permisos para asignar tickets"
                };
            }

            // Verificar que el admin al que se asigna existe
            var assignedAdmin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.AdminId);
            if (assignedAdmin == null || assignedAdmin.UserType != "Administrador")
            {
                return new AssignTicketResponse
                {
                    Success = false,
                    Message = "El administrador asignado no existe o no es válido"
                };
            }

            // Obtener el ticket
            var ticket = await _unitOfWork.Repository<Supportticket>().GetById(request.TicketId);
            if (ticket == null)
            {
                return new AssignTicketResponse
                {
                    Success = false,
                    Message = "Ticket no encontrado"
                };
            }

            // Asignar el ticket
            ticket.AssignedTo = request.AdminId;

            // Si estaba abierto, cambiar a "En Progreso"
            if (ticket.TicketStatus == "Abierto")
            {
                ticket.TicketStatus = "En Progreso";
            }

            await _unitOfWork.Repository<Supportticket>().Update(ticket);

            // Notificar al usuario del ticket
            await _notificationService.CreateNotificationAsync(
                ticket.UserId,
                "admin_action",
                "Ticket Asignado",
                $"Tu ticket #{ticket.TicketId} ha sido asignado y está siendo revisado",
                "ticket",
                ticket.TicketId
            );

            // Notificar al admin asignado
            await _notificationService.CreateNotificationAsync(
                request.AdminId,
                "admin_action",
                "Ticket Asignado",
                $"Se te ha asignado el ticket #{ticket.TicketId}: {ticket.Subject}",
                "ticket",
                ticket.TicketId
            );

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "AssignTicket",
                $"Asignó ticket #{request.TicketId} al admin #{request.AdminId}"
            );

            await _unitOfWork.Complete();

            return new AssignTicketResponse
            {
                Success = true,
                Message = "Ticket asignado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new AssignTicketResponse
            {
                Success = false,
                Message = $"Error al asignar ticket: {ex.Message}"
            };
        }
    }
}
