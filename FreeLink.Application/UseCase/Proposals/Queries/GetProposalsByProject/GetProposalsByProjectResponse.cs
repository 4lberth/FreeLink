using FreeLink.Application.UseCase.Proposals.DTOs;

namespace FreeLink.Application.UseCase.Proposals.Queries.GetProposalsByProject;

public class GetProposalsByProjectResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ProposalSummaryDto> Proposals { get; set; } = new();
}
