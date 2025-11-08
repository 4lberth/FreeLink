using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Projectactivitylog
{
    public int ActivityId { get; set; }

    public int ProjectId { get; set; }

    public int? UserId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? ActivityDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User? User { get; set; }
}
