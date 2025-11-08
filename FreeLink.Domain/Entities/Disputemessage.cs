using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Disputemessage
{
    public int DisputeMessageId { get; set; }

    public int DisputeId { get; set; }

    public int SenderId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public virtual Dispute Dispute { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
