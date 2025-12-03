using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.AssignTicket;

public class AssignTicketCommand : IRequest<AssignTicketResponse>
{
    public int TicketId { get; set; }
    public int AdminId { get; set; } // Admin to assign the ticket to
    public int RequestingAdminId { get; set; }
}
