using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ReplyToTicket;

public class ReplyToTicketCommand : IRequest<ReplyToTicketResponse>
{
    public int TicketId { get; set; }
    public int RequestingAdminId { get; set; }
    public string ResponseText { get; set; } = string.Empty;
}
