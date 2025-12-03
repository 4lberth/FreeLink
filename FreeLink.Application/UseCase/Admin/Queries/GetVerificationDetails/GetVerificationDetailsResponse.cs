namespace FreeLink.Application.UseCase.Admin.Queries.GetVerificationDetails;

public class GetVerificationDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public VerificationDetailsDto? Data { get; set; }
}

public class VerificationDetailsDto
{
    public int VerificationId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? UserType { get; set; }
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public string? DocumentFrontUrl { get; set; }
    public string? DocumentBackUrl { get; set; }
    public string? SelfieUrl { get; set; }
    public string? VerificationStatus { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
    public string? RejectionReason { get; set; }
}
