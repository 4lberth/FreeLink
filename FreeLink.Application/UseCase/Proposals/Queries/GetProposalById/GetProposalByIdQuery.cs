using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalById;

public class GetProposalByIdQuery : IRequest<GetProposalByIdResponse>
{
    public int ProposalId { get; set; }
    public int? RequestingUserId { get; set; }
}
