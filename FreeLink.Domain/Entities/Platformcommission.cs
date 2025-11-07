using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Platformcommission
{
    public int CommissionId { get; set; }

    public int TransactionId { get; set; }

    public int ProjectId { get; set; }

    public decimal Amount { get; set; }

    public decimal CommissionRate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
