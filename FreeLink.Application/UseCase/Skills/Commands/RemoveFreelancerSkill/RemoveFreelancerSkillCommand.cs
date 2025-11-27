using MediatR;

namespace FreeLink.Application.UseCase.Skills.Commands.RemoveFreelancerSkill;

public class RemoveFreelancerSkillCommand : IRequest<RemoveFreelancerSkillResponse>
{
    public int FreelancerId { get; set; }
    public int SkillId { get; set; }
    public int RequestingUserId { get; set; }
}
