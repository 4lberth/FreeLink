namespace FreeLink.Application.UseCase.Payments.Commands.RequestWithdrawal;

public class RequestWithdrawalResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? WithdrawalId { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
}
