using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.DeactivateUser;

public class DeactivateUserCommand : IRequest<DeactivateUserResponse>
{
    public int UserId { get; set; }
    public int RequestingUserId { get; set; }
}
