using MediatR;

namespace FreeLink.Application.UseCase.Freelancer.Commands.AddFreelancerSkill;

public class AddFreelancerSkillCommand : IRequest<AddFreelancerSkillResponse>
{
    public int FreelancerId { get; set; }
    public int SkillId { get; set; }
    
    // Usuario que hace la petición (desde el token)
    public int RequestingUserId { get; set; }
    public string RequestingUserRole { get; set; } = string.Empty;
}