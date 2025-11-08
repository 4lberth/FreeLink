using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.AddWorkExperience;

public class AddWorkExperienceCommand : IRequest<AddWorkExperienceResponse>
{
    public int UserId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string? Company { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

