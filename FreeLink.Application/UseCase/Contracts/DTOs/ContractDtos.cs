namespace FreeLink.Application.UseCase.Contracts.DTOs;

public class ContractDto
{
    public int ContractId { get; set; }
    public int ProposalId { get; set; }
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public int FreelancerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string ContractStatus { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime? ClientSignedAt { get; set; }
    public DateTime? FreelancerSignedAt { get; set; }
    public string? PdfUrl { get; set; }
}
