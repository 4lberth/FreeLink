namespace FreeLink.Application.UseCase.User.Commands.AddFreelancerSkill;

public class AddFreelancerSkillResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? FreelancerSkillId { get; set; }
}

