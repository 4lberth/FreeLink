namespace FreeLink.Application.UseCase.Proposals.Commands.AcceptProposal;

public class AcceptProposalResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ContractId { get; set; } // ID del contrato generado
}
