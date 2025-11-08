using MediatR;

namespace FreeLink.Application.UseCase.User.Queries.LoginUser;

public class LoginUserQuery : IRequest<LoginUserResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}