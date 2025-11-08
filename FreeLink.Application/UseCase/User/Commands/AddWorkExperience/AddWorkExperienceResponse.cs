namespace FreeLink.Application.UseCase.User.Commands.AddWorkExperience;

public class AddWorkExperienceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? ExperienceId { get; set; }
}

