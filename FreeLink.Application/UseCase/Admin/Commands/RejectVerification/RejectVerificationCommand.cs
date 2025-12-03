using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.RejectVerification;

public class RejectVerificationCommand : IRequest<RejectVerificationResponse>
{
    public int VerificationId { get; set; }
    public int RequestingAdminId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
