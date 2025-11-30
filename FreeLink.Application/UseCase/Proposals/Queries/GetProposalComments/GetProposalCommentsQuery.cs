using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalComments;

public class GetProposalCommentsQuery : IRequest<GetProposalCommentsResponse>
{
    public int ProposalId { get; set; }
    public int? RequestingUserId { get; set; }
}
