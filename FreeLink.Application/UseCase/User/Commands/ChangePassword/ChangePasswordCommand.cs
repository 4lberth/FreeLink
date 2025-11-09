using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<ChangePasswordResponse>
{
    public int UserId { get; set; }
    public string? CurrentPassword { get; set; }      // Null si admin resetea
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
    
    // Usuario que hace la petición (desde el token)
    public int RequestingUserId { get; set; }
    public string RequestingUserRole { get; set; } = string.Empty;
}