using FreeLink.Application.UseCase.Portfolio.DTOs;

namespace FreeLink.Application.UseCase.Portfolio.Queries.GetUserPortfolio;

public class GetUserPortfolioResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<PortfolioItemDto> Items { get; set; } = new();
}
