using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ApproveVerification;

public class ApproveVerificationCommand : IRequest<ApproveVerificationResponse>
{
    public int VerificationId { get; set; }
    public int RequestingAdminId { get; set; }
}
