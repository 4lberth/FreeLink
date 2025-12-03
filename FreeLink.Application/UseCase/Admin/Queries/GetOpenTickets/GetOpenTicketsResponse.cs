namespace FreeLink.Application.UseCase.Admin.Queries.GetOpenTickets;

public class GetOpenTicketsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<TicketListDto> Tickets { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class TicketListDto
{
    public int TicketId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? TicketStatus { get; set; }
    public string? Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
