namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingDisputes;

public class GetPendingDisputesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<DisputeListDto> Disputes { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class DisputeListDto
{
    public int DisputeId { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
    public int InitiatorId { get; set; }
    public string InitiatorName { get; set; } = string.Empty;
    public int RespondentId { get; set; }
    public string RespondentName { get; set; } = string.Empty;
    public string? DisputeReason { get; set; }
    public string? DisputeStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? MediatorId { get; set; }
    public string? MediatorName { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
