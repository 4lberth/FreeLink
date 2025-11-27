namespace FreeLink.Application.UseCase.Skills.Commands.CreateSkill;

public class CreateSkillResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? SkillId { get; set; }
}
