using FreeLink.Application.UseCase.Portfolio.DTOs;

namespace FreeLink.Application.UseCase.Portfolio.Queries.GetPortfolioItemById;

public class GetPortfolioItemByIdResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public PortfolioItemDto? Item { get; set; }
}
