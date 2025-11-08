namespace FreeLink.Application.UseCase.User.Commands.UpdateUser;

public class UpdateUserResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
}