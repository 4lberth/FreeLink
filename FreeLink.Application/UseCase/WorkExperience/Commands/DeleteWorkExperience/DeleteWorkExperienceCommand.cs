using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Commands.DeleteWorkExperience;

public class DeleteWorkExperienceCommand : IRequest<DeleteWorkExperienceResponse>
{
    public int ExperienceId { get; set; }
    public int RequestingUserId { get; set; }
}
