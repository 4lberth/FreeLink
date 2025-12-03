namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingWithdrawals;

public class GetPendingWithdrawalsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<WithdrawalDto> Withdrawals { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class WithdrawalDto
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Description { get; set; } = string.Empty;
}
