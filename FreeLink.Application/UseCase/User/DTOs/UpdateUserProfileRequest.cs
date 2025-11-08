namespace FreeLink.Application.UseCase.User.DTOs;

public class UpdateUserProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
}