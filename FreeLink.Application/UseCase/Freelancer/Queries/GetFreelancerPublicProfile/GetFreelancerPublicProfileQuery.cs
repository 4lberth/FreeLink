using MediatR;

namespace FreeLink.Application.UseCase.Freelancer.Queries.GetFreelancerPublicProfile;

public class GetFreelancerPublicProfileQuery : IRequest<GetFreelancerPublicProfileResponse>
{
    public int FreelancerId { get; set; }
}