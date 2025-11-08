using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Proposaldeliverable
{
    public int DeliverableId { get; set; }

    public int ProposalId { get; set; }

    public string DeliverableName { get; set; } = null!;

    public string? Description { get; set; }

    public int? ItemOrder { get; set; }

    public virtual Proposal Proposal { get; set; } = null!;
}
