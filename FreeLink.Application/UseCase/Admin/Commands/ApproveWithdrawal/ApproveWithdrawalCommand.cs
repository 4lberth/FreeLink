using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.ApproveWithdrawal;

public class ApproveWithdrawalCommand : IRequest<ApproveWithdrawalResponse>
{
    public int WithdrawalId { get; set; }
    public int AdminId { get; set; }
    public string? AdminNotes { get; set; }
}
