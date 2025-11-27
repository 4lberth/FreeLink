namespace FreeLink.Application.UseCase.Portfolio.DTOs;

public class PortfolioItemDto
{
    public int PortfolioId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PortfolioFileDto> Files { get; set; } = new();
}

public class PortfolioFileDto
{
    public int FileId { get; set; }
    public int PortfolioId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class CreatePortfolioItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
}

public class UpdatePortfolioItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
}
