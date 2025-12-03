using MediatR;

namespace FreeLink.Application.UseCase.Tickets.Commands.CreateTicket;

public class CreateTicketCommand : IRequest<CreateTicketResponse>
{
    public int UserId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Priority { get; set; }
}
