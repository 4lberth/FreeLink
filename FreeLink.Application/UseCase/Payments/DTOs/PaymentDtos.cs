namespace FreeLink.Application.UseCase.Payments.DTOs;

/// <summary>
/// DTO para cuenta de garantía (Escrow)
/// </summary>
public class EscrowDto
{
    public int EscrowId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = null!;
    public int ClientId { get; set; }
    public string ClientName { get; set; } = null!;
    public int FreelancerId { get; set; }
    public string FreelancerName { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string EscrowStatus { get; set; } = null!; // Pending, Deposited, Held, Released, Cancelled
    public DateTime? DepositedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}

/// <summary>
/// DTO para transacciones
/// </summary>
public class TransactionDto
{
    public int TransactionId { get; set; }
    public int? EscrowId { get; set; }
    public int? FromUserId { get; set; }
    public string? FromUserName { get; set; }
    public int? ToUserId { get; set; }
    public string? ToUserName { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = null!; // Deposit, Release, Commission, Withdrawal
    public string TransactionStatus { get; set; } = null!; // Pending, Completed, Failed
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? ProjectTitle { get; set; }
}

/// <summary>
/// DTO para comisiones de la plataforma
/// </summary>
public class CommissionDto
{
    public int CommissionId { get; set; }
    public int TransactionId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal CommissionRate { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para balance del usuario
/// </summary>
public class BalanceDto
{
    public int UserId { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal TotalSpent { get; set; }
    public int CompletedTransactions { get; set; }
    public int PendingTransactions { get; set; }
}

/// <summary>
/// DTO para comprobante de pago
/// </summary>
public class PaymentReceiptDto
{
    public int TransactionId { get; set; }
    public string ReceiptNumber { get; set; } = null!;
    public DateTime IssuedDate { get; set; }
    public string FromUserName { get; set; } = null!;
    public string ToUserName { get; set; } = null!;
    public string ProjectTitle { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public string ReceiptUrl { get; set; } = null!;
}

/// <summary>
/// DTO para resumen de transacción (usado en listados)
/// </summary>
public class TransactionSummaryDto
{
    public int TransactionId { get; set; }
    public string TransactionType { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
