using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Proposalcostbreakdown
{
    public int CostItemId { get; set; }

    public int ProposalId { get; set; }

    public string ItemDescription { get; set; } = null!;

    public decimal Amount { get; set; }

    public int? ItemOrder { get; set; }

    public virtual Proposal Proposal { get; set; } = null!;
}
