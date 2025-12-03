using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.UpdateTicketStatus;

public class UpdateTicketStatusCommand : IRequest<UpdateTicketStatusResponse>
{
    public int TicketId { get; set; }
    public int RequestingAdminId { get; set; }
    public string NewStatus { get; set; } = string.Empty; // "Abierto", "En Progreso", "Resuelto", "Cerrado"
}
