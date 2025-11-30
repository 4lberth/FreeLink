using FreeLink.Application.UseCase.Proposals.DTOs;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalComments;

public class GetProposalCommentsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ProposalCommentDto> Comments { get; set; } = new();
}
