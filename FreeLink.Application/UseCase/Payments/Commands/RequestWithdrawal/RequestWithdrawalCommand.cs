using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.RequestWithdrawal;

public class RequestWithdrawalCommand : IRequest<RequestWithdrawalResponse>
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}
