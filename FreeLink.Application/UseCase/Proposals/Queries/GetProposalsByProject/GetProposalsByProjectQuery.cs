using MediatR;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalsByProject;

public class GetProposalsByProjectQuery : IRequest<GetProposalsByProjectResponse>
{
    public int ProjectId { get; set; }
    public int? RequestingUserId { get; set; }
}
