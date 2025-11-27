using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ActivateUser;

public class ActivateUserCommand : IRequest<ActivateUserResponse>
{
    public int UserId { get; set; }
    public int RequestingUserId { get; set; }
}
