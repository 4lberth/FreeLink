using System;
using System.Collections.Generic;

namespace FreeLink.Domain.Entities; 

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? EscrowId { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = null!;

    public string? TransactionStatus { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ReceiptUrl { get; set; }

    public virtual Escrowaccount? Escrow { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual ICollection<Platformcommission> Platformcommissions { get; set; } = new List<Platformcommission>();

    public virtual User? ToUser { get; set; }
}
