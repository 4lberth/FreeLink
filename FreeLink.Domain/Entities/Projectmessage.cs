using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Projectmessage
{
    public int MessageId { get; set; }

    public int ProjectId { get; set; }

    public int SenderId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual ICollection<Contentreport> Contentreports { get; set; } = new List<Contentreport>();

    public virtual ICollection<Messageattachment> Messageattachments { get; set; } = new List<Messageattachment>();

    public virtual Project Project { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
