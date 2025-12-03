using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ReplyToTicket;

public class ReplyToTicketHandler : IRequestHandler<ReplyToTicketCommand, ReplyToTicketResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IAdminActivityLogger _activityLogger;

    public ReplyToTicketHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAdminActivityLogger activityLogger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _activityLogger = activityLogger;
    }

    public async Task<ReplyToTicketResponse> Handle(ReplyToTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new ReplyToTicketResponse
                {
                    Success = false,
                    Message = "No tienes permisos para responder tickets"
                };
            }

            // Obtener el ticket
            var ticket = await _unitOfWork.Repository<Supportticket>().GetById(request.TicketId);
            if (ticket == null)
            {
                return new ReplyToTicketResponse
                {
                    Success = false,
                    Message = "Ticket no encontrado"
                };
            }

            // Validar respuesta
            if (string.IsNullOrWhiteSpace(request.ResponseText))
            {
                return new ReplyToTicketResponse
                {
                    Success = false,
                    Message = "Debe proporcionar un mensaje de respuesta"
                };
            }

            // Crear la respuesta
            var response = new Ticketresponse
            {
                TicketId = request.TicketId,
                ResponderId = request.RequestingAdminId,
                ResponseText = request.ResponseText,
                IsStaffResponse = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Ticketresponse>().Add(response);

            // Si el ticket está abierto, ponerlo en progreso
            if (ticket.TicketStatus == "Abierto")
            {
                ticket.TicketStatus = "En Progreso";
                await _unitOfWork.Repository<Supportticket>().Update(ticket);
            }

            // Notificar al usuario del ticket
            await _notificationService.CreateNotificationAsync(
                ticket.UserId,
                "admin_action",
                "Nueva Respuesta en tu Ticket",
                $"El soporte ha respondido a tu ticket #{ticket.TicketId}",
                "ticket",
                ticket.TicketId
            );

            // Registrar actividad
            await _activityLogger.LogActivity(
                request.RequestingAdminId,
                "ReplyToTicket",
                $"Respondió al ticket #{request.TicketId}"
            );

            await _unitOfWork.Complete();

            return new ReplyToTicketResponse
            {
                Success = true,
                Message = "Respuesta enviada exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ReplyToTicketResponse
            {
                Success = false,
                Message = $"Error al responder ticket: {ex.Message}"
            };
        }
    }
}
