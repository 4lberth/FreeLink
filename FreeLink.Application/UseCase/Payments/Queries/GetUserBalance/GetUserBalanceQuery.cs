using MediatR;

namespace FreeLink.Application.UseCase.Payments.Queries.GetUserBalance;

public class GetUserBalanceQuery : IRequest<GetUserBalanceResponse>
{
    public int UserId { get; set; }
}
