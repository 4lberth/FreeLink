namespace FreeLink.Application.UseCase.Admin.Queries.GetTransactionDetails;

public class GetTransactionDetailsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TransactionDetailsDto? Data { get; set; }
}

public class TransactionDetailsDto
{
    public int TransactionId { get; set; }
    public int FromUserId { get; set; }
    public string FromUserName { get; set; } = string.Empty;
    public string FromUserEmail { get; set; } = string.Empty;
    public int ToUserId { get; set; }
    public string ToUserName { get; set; } = string.Empty;
    public string ToUserEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
}
