using FreeLink.Application.UseCase.Portfolio.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Queries.GetPortfolioItemById;

public class GetPortfolioItemByIdQuery : IRequest<GetPortfolioItemByIdResponse>
{
    public int PortfolioId { get; set; }
}
