using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.RejectApplication;

public class RejectApplicationCommand : IRequest<RejectApplicationResponse>
{
    public int ApplicationId { get; set; }
    public int RequestingUserId { get; set; }
}
