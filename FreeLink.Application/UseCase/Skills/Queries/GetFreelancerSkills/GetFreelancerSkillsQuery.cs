using MediatR;

namespace FreeLink.Application.UseCase.Skills.Queries.GetFreelancerSkills;

public class GetFreelancerSkillsQuery : IRequest<GetFreelancerSkillsResponse>
{
    public int FreelancerId { get; set; }
}
