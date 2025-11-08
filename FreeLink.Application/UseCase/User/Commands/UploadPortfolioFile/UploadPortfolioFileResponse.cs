namespace FreeLink.Application.UseCase.User.Commands.UploadPortfolioFile;

public class UploadPortfolioFileResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? PortfolioId { get; set; }
    public List<string> FileUrls { get; set; } = new();
}

