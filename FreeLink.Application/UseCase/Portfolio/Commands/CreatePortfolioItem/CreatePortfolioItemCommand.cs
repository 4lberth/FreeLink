using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.CreatePortfolioItem;

public class CreatePortfolioItemCommand : IRequest<CreatePortfolioItemResponse>
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProjectUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public int RequestingUserId { get; set; }
}
