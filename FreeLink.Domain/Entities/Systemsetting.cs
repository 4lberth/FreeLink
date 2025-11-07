using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Systemsetting
{
    public int SettingId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string SettingValue { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
