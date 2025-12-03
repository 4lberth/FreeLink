using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.User.DTOs;

public class UploadProfilePictureRequest
{
    public IFormFile File { get; set; } = null!;
}