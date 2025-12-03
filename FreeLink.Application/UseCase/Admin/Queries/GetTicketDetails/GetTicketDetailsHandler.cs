using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetTicketDetails;

public class GetTicketDetailsHandler : IRequestHandler<GetTicketDetailsQuery, GetTicketDetailsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketDetailsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetTicketDetailsResponse> Handle(GetTicketDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetTicketDetailsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver detalles de tickets"
                };
            }

            // Obtener el ticket
            var ticket = await _unitOfWork.Repository<Supportticket>().GetById(request.TicketId);
            if (ticket == null)
            {
                return new GetTicketDetailsResponse
                {
                    Success = false,
                    Message = "Ticket no encontrado"
                };
            }

            // Obtener usuario del ticket
            var user = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(ticket.UserId);

            // Obtener admin asignado si existe
            FreeLink.Domain.Entities.User? assignedToUser = null;
            if (ticket.AssignedTo.HasValue)
            {
                assignedToUser = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(ticket.AssignedTo.Value);
            }

            // Obtener todas las respuestas del ticket
            var allResponses = await _unitOfWork.Repository<Ticketresponse>().GetAll();
            var ticketResponses = allResponses
                .Where(r => r.TicketId == request.TicketId)
                .OrderBy(r => r.CreatedAt)
                .ToList();

            // Obtener usuarios que respondieron
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear respuestas a DTOs
            var responseDtos = ticketResponses.Select(r =>
            {
                var responder = usersList.FirstOrDefault(u => u.UserId == r.ResponderId);
                return new TicketResponseDto
                {
                    ResponseId = r.ResponseId,
                    ResponderId = r.ResponderId,
                    ResponderName = responder?.Email ?? "Usuario no encontrado",
                    ResponseText = r.ResponseText,
                    IsStaffResponse = r.IsStaffResponse,
                    CreatedAt = r.CreatedAt
                };
            }).ToList();

            var ticketDetails = new TicketDetailsDto
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                UserName = user?.Email ?? "Usuario no encontrado",
                UserEmail = user?.Email ?? "",
                Subject = ticket.Subject,
                Description = ticket.Description,
                TicketStatus = ticket.TicketStatus,
                Priority = ticket.Priority,
                CreatedAt = ticket.CreatedAt,
                AssignedTo = ticket.AssignedTo,
                AssignedToName = assignedToUser?.Email,
                ResolvedAt = ticket.ResolvedAt,
                Responses = responseDtos
            };

            return new GetTicketDetailsResponse
            {
                Success = true,
                Message = "Detalles del ticket obtenidos exitosamente",
                Data = ticketDetails
            };
        }
        catch (Exception ex)
        {
            return new GetTicketDetailsResponse
            {
                Success = false,
                Message = $"Error al obtener detalles del ticket: {ex.Message}"
            };
        }
    }
}
