using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.CompleteProject;

public class CompleteProjectCommand : IRequest<CompleteProjectResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
