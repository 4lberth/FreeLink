using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Escrowaccount
{
    public int EscrowId { get; set; }

    public int ProjectId { get; set; }

    public int ClientId { get; set; }

    public int FreelancerId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? EscrowStatus { get; set; }

    public DateTime? DepositedAt { get; set; }

    public DateTime? ReleasedAt { get; set; }

    public virtual User Client { get; set; } = null!;

    public virtual User Freelancer { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
