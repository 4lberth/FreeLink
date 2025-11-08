using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.GetUserProfile;

public class GetUserProfileQuery : IRequest<GetUserProfileResponse>
{
    public int UserId { get; set; }
}