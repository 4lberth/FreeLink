namespace FreeLink.Application.UseCase.User.DTOs;

public class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? UserType { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsVerified { get; set; }
}