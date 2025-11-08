using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Proposal
{
    public int ProposalId { get; set; }

    public int ProjectId { get; set; }

    public int FreelancerId { get; set; }

    public int? VersionNumber { get; set; }

    public decimal TotalCost { get; set; }

    public string? ProposalStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual User Freelancer { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<Proposalcomment> Proposalcomments { get; set; } = new List<Proposalcomment>();

    public virtual ICollection<Proposalcostbreakdown> Proposalcostbreakdowns { get; set; } = new List<Proposalcostbreakdown>();

    public virtual ICollection<Proposaldeliverable> Proposaldeliverables { get; set; } = new List<Proposaldeliverable>();

    public virtual ICollection<Proposaltimeline> Proposaltimelines { get; set; } = new List<Proposaltimeline>();
}
