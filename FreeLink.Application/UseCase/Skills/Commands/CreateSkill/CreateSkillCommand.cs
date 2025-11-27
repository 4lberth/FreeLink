using MediatR;

namespace FreeLink.Application.UseCase.Skills.Commands.CreateSkill;

public class CreateSkillCommand : IRequest<CreateSkillResponse>
{
    public string SkillName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int RequestingUserId { get; set; }
}
