using FreeLink.Application.UseCase.Payments.DTOs;

namespace FreeLink.Application.UseCase.Payments.Queries.GetUserBalance;

public class GetUserBalanceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public BalanceDto? Balance { get; set; }
}
