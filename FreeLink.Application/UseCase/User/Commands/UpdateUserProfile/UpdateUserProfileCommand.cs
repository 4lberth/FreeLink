using MediatR;

namespace FreeLink.Application.UseCase.User.Commands.UpdateUserProfile;


public class UpdateUserProfileCommand : IRequest<UpdateUserProfileResponse>
{
    public int UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    
    // Usuario que hace la petición (desde el token)
    public int RequestingUserId { get; set; }
    public string RequestingUserRole { get; set; } = string.Empty;
}