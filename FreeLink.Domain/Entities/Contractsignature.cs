using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Contractsignature
{
    public int SignatureId { get; set; }

    public int ContractId { get; set; }

    public int UserId { get; set; }

    public string? SignatureData { get; set; }

    public string? IpAddress { get; set; }

    public DateTime SignedAt { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
