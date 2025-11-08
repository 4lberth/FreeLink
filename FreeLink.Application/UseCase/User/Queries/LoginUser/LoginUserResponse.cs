using FreeLink.Application.UseCase.User.DTOs;

namespace FreeLink.Application.UseCase.User.Queries.LoginUser;

public class LoginUserResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public UserDto? User { get; set; }
}