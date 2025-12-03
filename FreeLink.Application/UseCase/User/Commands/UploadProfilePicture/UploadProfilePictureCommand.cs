using MediatR;
using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.User.Commands.UploadProfilePicture;

public class UploadProfilePictureCommand : IRequest<UploadProfilePictureResponse>
{
    public int UserId { get; set; }
    public IFormFile File { get; set; } = null!;
}
