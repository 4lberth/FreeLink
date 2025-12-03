namespace FreeLink.Application.UseCase.Admin.Queries.GetTicketDetails;

public class GetTicketDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TicketDetailsDto? Data { get; set; }
}

public class TicketDetailsDto
{
    public int TicketId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public string? TicketStatus { get; set; }
    public string? Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<TicketResponseDto> Responses { get; set; } = new();
}

public class TicketResponseDto
{
    public int ResponseId { get; set; }
    public int ResponderId { get; set; }
    public string ResponderName { get; set; } = string.Empty;
    public string? ResponseText { get; set; }
    public bool? IsStaffResponse { get; set; }
    public DateTime CreatedAt { get; set; }
}
