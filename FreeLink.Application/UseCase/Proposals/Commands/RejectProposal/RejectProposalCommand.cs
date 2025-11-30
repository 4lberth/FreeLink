using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.RejectProposal;

public class RejectProposalCommand : IRequest<RejectProposalResponse>
{
    public int ProposalId { get; set; }
    public int RequestingUserId { get; set; } // Cliente
    public string? RejectionReason { get; set; } // Opcional
}
