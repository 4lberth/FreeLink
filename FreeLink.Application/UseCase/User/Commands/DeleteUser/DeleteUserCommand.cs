using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<DeleteUserResponse>
{
    public int UserId { get; set; }
}

