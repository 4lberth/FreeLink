using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Contract
{
    public int ContractId { get; set; }

    public int ProjectId { get; set; }

    public int ProposalId { get; set; }

    public int ClientId { get; set; }

    public int FreelancerId { get; set; }

    public string? ContractStatus { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime GeneratedAt { get; set; }

    public DateTime? ClientSignedAt { get; set; }

    public DateTime? FreelancerSignedAt { get; set; }

    public string? ContractPdfUrl { get; set; }

    public virtual User Client { get; set; } = null!;

    public virtual ICollection<Contractsignature> Contractsignatures { get; set; } = new List<Contractsignature>();

    public virtual User Freelancer { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual Proposal Proposal { get; set; } = null!;
}
