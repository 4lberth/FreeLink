namespace FreeLink.Application.UseCase.Admin.Commands.ApproveWithdrawal;

public class ApproveWithdrawalResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? WithdrawalId { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
}
