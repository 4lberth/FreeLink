using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetOpenTickets;

public class GetOpenTicketsHandler : IRequestHandler<GetOpenTicketsQuery, GetOpenTicketsResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOpenTicketsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetOpenTicketsResponse> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar que el solicitante es administrador
            var admin = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetById(request.RequestingAdminId);
            if (admin == null || admin.UserType != "Administrador")
            {
                return new GetOpenTicketsResponse
                {
                    Success = false,
                    Message = "No tienes permisos para ver tickets"
                };
            }

            // Obtener todos los tickets
            var allTickets = await _unitOfWork.Repository<Supportticket>().GetAll();
            var tickets = allTickets.AsEnumerable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(request.Status))
            {
                tickets = tickets.Where(t => t.TicketStatus == request.Status);
            }
            else
            {
                // Por defecto, solo tickets abiertos o en progreso
                tickets = tickets.Where(t => t.TicketStatus == "Abierto" || t.TicketStatus == "En Progreso");
            }

            if (!string.IsNullOrEmpty(request.Priority))
            {
                tickets = tickets.Where(t => t.Priority == request.Priority);
            }

            tickets = tickets.OrderByDescending(t => t.CreatedAt);

            var totalCount = tickets.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var pagedTickets = tickets
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Obtener usuarios
            var users = await _unitOfWork.Repository<FreeLink.Domain.Entities.User>().GetAll();
            var usersList = users.ToList();

            // Mapear a DTOs
            var ticketDtos = pagedTickets.Select(t =>
            {
                var user = usersList.FirstOrDefault(u => u.UserId == t.UserId);
                var assignedToUser = t.AssignedTo.HasValue ? usersList.FirstOrDefault(u => u.UserId == t.AssignedTo.Value) : null;

                return new TicketListDto
                {
                    TicketId = t.TicketId,
                    UserId = t.UserId,
                    UserName = user?.Email ?? "Usuario no encontrado",
                    UserEmail = user?.Email ?? "",
                    Subject = t.Subject,
                    TicketStatus = t.TicketStatus,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    AssignedTo = t.AssignedTo,
                    AssignedToName = assignedToUser?.Email,
                    ResolvedAt = t.ResolvedAt
                };
            }).ToList();

            return new GetOpenTicketsResponse
            {
                Success = true,
                Message = "Tickets obtenidos exitosamente",
                Tickets = ticketDtos,
                TotalCount = totalCount,
                CurrentPage = request.Page,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            return new GetOpenTicketsResponse
            {
                Success = false,
                Message = $"Error al obtener tickets: {ex.Message}"
            };
        }
    }
}
