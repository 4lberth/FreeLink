using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.UpdatePortfolioItem;

public class UpdatePortfolioItemCommand : IRequest<UpdatePortfolioItemResponse>
{
    public int PortfolioId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public int RequestingUserId { get; set; }
}
