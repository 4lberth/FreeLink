using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetFreelancerProfile;

public class GetFreelancerProfileQuery : IRequest<GetFreelancerProfileResponse>
{
    public int UserId { get; set; }
}

