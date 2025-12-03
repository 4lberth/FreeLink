namespace FreeLink.Application.UseCase.Admin.Queries.GetDisputeDetails;

public class GetDisputeDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DisputeDetailsDto? Data { get; set; }
}

public class DisputeDetailsDto
{
    public int DisputeId { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
    public int InitiatorId { get; set; }
    public string InitiatorName { get; set; } = string.Empty;
    public string InitiatorEmail { get; set; } = string.Empty;
    public int RespondentId { get; set; }
    public string RespondentName { get; set; } = string.Empty;
    public string RespondentEmail { get; set; } = string.Empty;
    public string? DisputeReason { get; set; }
    public string? DisputeDescription { get; set; }
    public string? DisputeStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MediatorId { get; set; }
    public string? MediatorName { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Resolution { get; set; }
    public List<DisputeMessageDto> Messages { get; set; } = new();
}

public class DisputeMessageDto
{
    public int DisputeMessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string? MessageText { get; set; }
    public DateTime SentAt { get; set; }
}
