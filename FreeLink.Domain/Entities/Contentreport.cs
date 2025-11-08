using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Contentreport
{
    public int ReportId { get; set; }

    public int ReporterId { get; set; }

    public int? ReportedUserId { get; set; }

    public int? ReportedProjectId { get; set; }

    public int? ReportedMessageId { get; set; }

    public string ReportReason { get; set; } = null!;

    public string? ReportDescription { get; set; }

    public string? ReportStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int? ReviewedBy { get; set; }

    public string? Resolution { get; set; }

    public virtual Projectmessage? ReportedMessage { get; set; }

    public virtual Project? ReportedProject { get; set; }

    public virtual User? ReportedUser { get; set; }

    public virtual User Reporter { get; set; } = null!;

    public virtual User? ReviewedByNavigation { get; set; }
}
