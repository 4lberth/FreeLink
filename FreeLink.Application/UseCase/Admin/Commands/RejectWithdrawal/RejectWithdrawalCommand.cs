using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.RejectWithdrawal;

public class RejectWithdrawalCommand : IRequest<RejectWithdrawalResponse>
{
    public int WithdrawalId { get; set; }
    public int AdminId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
