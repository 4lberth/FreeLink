using FreeLink.Application.UseCase.Portfolio.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Portfolio.Queries.GetUserPortfolio;

public class GetUserPortfolioQuery : IRequest<GetUserPortfolioResponse>
{
    public int UserId { get; set; }
}
