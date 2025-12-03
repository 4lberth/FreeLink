namespace FreeLink.Application.UseCase.Admin.Queries.GetTransactionStatistics;

public class GetTransactionStatisticsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TransactionStatisticsDto? Data { get; set; }
}

public class TransactionStatisticsDto
{
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public int SuccessfulTransactions { get; set; }
    public int PendingTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public Dictionary<string, int> TransactionsByType { get; set; } = new();
    public Dictionary<string, decimal> AmountByType { get; set; } = new();
}
