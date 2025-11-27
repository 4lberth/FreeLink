namespace FreeLink.Application.UseCase.Portfolio.Commands.CreatePortfolioItem;

public class CreatePortfolioItemResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? PortfolioId { get; set; }
}
