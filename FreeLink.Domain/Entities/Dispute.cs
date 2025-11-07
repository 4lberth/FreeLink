using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Dispute
{
    public int DisputeId { get; set; }

    public int ProjectId { get; set; }

    public int InitiatorId { get; set; }

    public int RespondentId { get; set; }

    public string DisputeReason { get; set; } = null!;

    public string DisputeDescription { get; set; } = null!;

    public string? DisputeStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? MediatorId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? Resolution { get; set; }

    public virtual ICollection<Disputemessage> Disputemessages { get; set; } = new List<Disputemessage>();

    public virtual User Initiator { get; set; } = null!;

    public virtual User? Mediator { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User Respondent { get; set; } = null!;
}
