namespace FreeLink.Application.UseCase.Tickets.Commands.CreateTicket;

public class CreateTicketResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? TicketId { get; set; }
    public string? TicketStatus { get; set; }
}
