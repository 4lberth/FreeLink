namespace FreeLink.Application.UseCase.Admin.Queries.GetAllTransactions;

public class GetAllTransactionsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<TransactionListDto> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class TransactionListDto
{
    public int TransactionId { get; set; }
    public int FromUserId { get; set; }
    public string FromUserName { get; set; } = string.Empty;
    public int ToUserId { get; set; }
    public string ToUserName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
}
