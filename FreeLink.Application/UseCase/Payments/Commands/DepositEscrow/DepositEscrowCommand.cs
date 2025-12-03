using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.DepositEscrow;

public class DepositEscrowCommand : IRequest<DepositEscrowResponse>
{
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public decimal Amount { get; set; }
}
