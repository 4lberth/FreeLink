using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.StartProject;

public class StartProjectCommand : IRequest<StartProjectResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
