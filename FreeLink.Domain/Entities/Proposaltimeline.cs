using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Proposaltimeline
{
    public int TimelineId { get; set; }

    public int ProposalId { get; set; }

    public string MilestoneName { get; set; } = null!;

    public string? Description { get; set; }

    public int? EstimatedDuration { get; set; }

    public int? ItemOrder { get; set; }

    public virtual Proposal Proposal { get; set; } = null!;
}
