using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.AcceptApplication;

public class AcceptApplicationCommand : IRequest<AcceptApplicationResponse>
{
    public int ApplicationId { get; set; }
    public int RequestingUserId { get; set; }
}
