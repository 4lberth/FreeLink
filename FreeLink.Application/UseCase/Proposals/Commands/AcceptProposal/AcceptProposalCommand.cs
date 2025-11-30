using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Commands.AcceptProposal;

public class AcceptProposalCommand : IRequest<AcceptProposalResponse>
{
    public int ProposalId { get; set; }
    public int RequestingUserId { get; set; } // Cliente dueño del proyecto
}
