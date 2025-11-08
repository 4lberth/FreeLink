using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Userwallet
{
    public int WalletId { get; set; }

    public int UserId { get; set; }

    public decimal? Balance { get; set; }

    public decimal? PendingBalance { get; set; }

    public decimal? TotalEarnings { get; set; }

    public decimal? TotalSpent { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
