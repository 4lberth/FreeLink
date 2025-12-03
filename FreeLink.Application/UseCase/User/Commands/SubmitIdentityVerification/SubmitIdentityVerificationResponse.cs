namespace FreeLink.Application.UseCase.User.Commands.SubmitIdentityVerification;

public class SubmitIdentityVerificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? VerificationId { get; set; }
    public string? Status { get; set; }
}
