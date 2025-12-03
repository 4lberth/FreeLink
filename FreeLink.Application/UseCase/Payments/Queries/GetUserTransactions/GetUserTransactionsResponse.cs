using FreeLink.Application.UseCase.Payments.DTOs;

namespace FreeLink.Application.UseCase.Payments.Queries.GetUserTransactions;

public class GetUserTransactionsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public decimal Balance { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
}
