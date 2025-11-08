using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.RemoveFreelancerSkill;

public class RemoveFreelancerSkillCommand : IRequest<RemoveFreelancerSkillResponse>
{
    public int UserId { get; set; }
    public int SkillId { get; set; }
}

