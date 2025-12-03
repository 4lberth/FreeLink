namespace FreeLink.Application.UseCase.User.Commands.UploadProfilePicture;

public class UploadProfilePictureResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
}
