namespace FreeLink.Application.UseCase.Admin.Queries.GetAllVerifications;

public class GetAllVerificationsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<VerificationDto> Verifications { get; set; } = new();
}

public class VerificationDto
{
    public int VerificationId { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentFrontUrl { get; set; } = string.Empty;
    public string DocumentBackUrl { get; set; } = string.Empty;
    public string SelfieUrl { get; set; } = string.Empty;
    public string VerificationStatus { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewedByEmail { get; set; }
    public string? RejectionReason { get; set; }
}
