using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<DeleteUserResponse>
{
    public int UserId { get; set; }
    public int RequestingUserId { get; set; }
}
