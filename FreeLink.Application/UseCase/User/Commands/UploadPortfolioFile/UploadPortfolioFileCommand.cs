using MediatR;
using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.User.Commands.UploadPortfolioFile;

public class UploadPortfolioFileCommand : IRequest<UploadPortfolioFileResponse>
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public IFormFile? ThumbnailFile { get; set; }
    public List<IFormFile>? Files { get; set; }
}

