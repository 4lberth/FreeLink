using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Adminactivitylog
{
    public int LogId { get; set; }

    public int AdminId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? ActionDescription { get; set; }

    public int? TargetUserId { get; set; }

    public string? TargetResourceType { get; set; }

    public int? TargetResourceId { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Admin { get; set; } = null!;

    public virtual User? TargetUser { get; set; }
}
