namespace FreeLink.Application.UseCase.Portfolio.Commands.UploadPortfolioFile;

public class UploadPortfolioFileResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? FileId { get; set; }
    public string? FileUrl { get; set; }
}
