using MediatR;
using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.User.Commands.SubmitIdentityVerification;

public class SubmitIdentityVerificationCommand : IRequest<SubmitIdentityVerificationResponse>
{
    public int UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public IFormFile? DocumentFront { get; set; }
    public IFormFile? DocumentBack { get; set; }
    public IFormFile? Selfie { get; set; }
}
