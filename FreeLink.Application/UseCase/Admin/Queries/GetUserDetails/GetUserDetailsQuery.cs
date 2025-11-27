using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetUserDetails;

public class GetUserDetailsQuery : IRequest<GetUserDetailsResponse>
{
    public int UserId { get; set; }
    public int RequestingUserId { get; set; }
}
