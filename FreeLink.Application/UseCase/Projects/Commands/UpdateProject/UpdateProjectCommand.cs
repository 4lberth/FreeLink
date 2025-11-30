using MediatR;

namespace FreeLink.Application.UseCase.Projects.Commands.UpdateProject;

public class UpdateProjectCommand : IRequest<UpdateProjectResponse>
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public DateOnly DeadlineDate { get; set; }
    public List<int> RequiredSkillIds { get; set; } = new();
    public int RequestingUserId { get; set; }
}
