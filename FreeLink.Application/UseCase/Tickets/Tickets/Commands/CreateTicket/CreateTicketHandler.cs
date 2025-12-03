using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Tickets.Commands.CreateTicket;

public class CreateTicketHandler : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public CreateTicketHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<CreateTicketResponse> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el usuario existe
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.UserId);
            if (user == null)
            {
                return new CreateTicketResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                };
            }

            // 2. Validar prioridad si se proporciona
            if (!string.IsNullOrEmpty(request.Priority))
            {
                var validPriorities = new[] { "Baja", "Media", "Alta" };
                if (!validPriorities.Contains(request.Priority))
                {
                    return new CreateTicketResponse
                    {
                        Success = false,
                        Message = "Prioridad inválida. Debe ser: Baja, Media o Alta"
                    };
                }
            }

            // 3. Crear el ticket
            var ticket = new Supportticket
            {
                UserId = request.UserId,
                Subject = request.Subject,
                Description = request.Description,
                Priority = request.Priority ?? "Media",
                TicketStatus = "Abierto",
                CreatedAt = DateTime.UtcNow,
                AssignedTo = null,
                ResolvedAt = null
            };

            await _unitOfWork.Repository<Supportticket>().Add(ticket);
            await _unitOfWork.Complete();

            // 4. Notificar al usuario
            await _notificationService.CreateNotificationAsync(
                userId: request.UserId,
                type: "TicketCreated",
                title: "Ticket Creado",
                message: $"Tu ticket de soporte #{ticket.TicketId} ha sido creado. Nuestro equipo lo revisará pronto.",
                resourceType: "Ticket",
                resourceId: ticket.TicketId
            );

            // 5. Notificar a todos los administradores
            var admins = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>()
                .GetAsync(u => u.UserType == "Administrador");
            var adminIds = admins.Select(a => a.UserId).ToList();

            if (adminIds.Any())
            {
                await _notificationService.CreateMultipleNotificationsAsync(
                    userIds: adminIds,
                    type: "NewTicketPending",
                    title: "Nuevo Ticket de Soporte",
                    message: $"El usuario {user.Email} creó un ticket: {request.Subject}. Prioridad: {ticket.Priority}",
                    resourceType: "Ticket",
                    resourceId: ticket.TicketId
                );
            }

            return new CreateTicketResponse
            {
                Success = true,
                Message = "Ticket creado exitosamente. Nuestro equipo te responderá pronto.",
                TicketId = ticket.TicketId,
                TicketStatus = "Abierto"
            };
        }
        catch (Exception ex)
        {
            return new CreateTicketResponse
            {
                Success = false,
                Message = $"Error al crear ticket: {ex.Message}"
            };
        }
    }
}
