using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.DeleteProject;

public class DeleteProjectCommand : IRequest<DeleteProjectResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
