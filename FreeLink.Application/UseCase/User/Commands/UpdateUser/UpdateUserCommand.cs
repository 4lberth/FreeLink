using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? UserType { get; set; }  // Solo admin puede cambiar
    public bool? IsActive { get; set; }     // Solo admin puede cambiar
    public bool? IsVerified { get; set; }   // Solo admin puede cambiar
    
    // Usuario que hace la petición (desde el token)
    public int RequestingUserId { get; set; }
    public string RequestingUserRole { get; set; } = string.Empty;
}