namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;

public class GetPendingVerificationsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<VerificationListDto> Verifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class VerificationListDto
{
    public int VerificationId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public string? VerificationStatus { get; set; }
    public DateTime SubmittedAt { get; set; }
}
