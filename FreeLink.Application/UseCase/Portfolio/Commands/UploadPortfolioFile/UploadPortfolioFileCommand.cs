using MediatR;
using Microsoft.AspNetCore.Http;

namespace FreeLink.Application.UseCase.Portfolio.Commands.UploadPortfolioFile;

public class UploadPortfolioFileCommand : IRequest<UploadPortfolioFileResponse>
{
    public int PortfolioId { get; set; }
    public IFormFile File { get; set; } = null!;
    public string FileType { get; set; } = string.Empty;
    public int RequestingUserId { get; set; }
}
