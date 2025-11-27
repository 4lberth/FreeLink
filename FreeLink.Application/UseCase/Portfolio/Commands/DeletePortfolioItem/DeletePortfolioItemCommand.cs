using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Commands.DeletePortfolioItem;

public class DeletePortfolioItemCommand : IRequest<DeletePortfolioItemResponse>
{
    public int PortfolioId { get; set; }
    public int RequestingUserId { get; set; }
}
