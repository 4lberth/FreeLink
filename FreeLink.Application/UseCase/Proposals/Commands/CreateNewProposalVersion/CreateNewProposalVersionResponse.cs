namespace FreeLink.Application.UseCase.Proposals.Commands.CreateNewProposalVersion;

public class CreateNewProposalVersionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ProposalId { get; set; }
    public int VersionNumber { get; set; }
}
