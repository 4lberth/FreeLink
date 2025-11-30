using FreeLink.Application.UseCase.Proposals.DTOs;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalById;

public class GetProposalByIdResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ProposalDto? Proposal { get; set; }
}
