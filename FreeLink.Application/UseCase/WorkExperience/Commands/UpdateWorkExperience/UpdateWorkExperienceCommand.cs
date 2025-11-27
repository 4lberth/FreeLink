using MediatR;

namespace FreeLink.Application.UseCase.WorkExperience.Commands.UpdateWorkExperience;

public class UpdateWorkExperienceCommand : IRequest<UpdateWorkExperienceResponse>
{
    public int ExperienceId { get; set; }
    public int UserId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string? Company { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public int RequestingUserId { get; set; }
}
