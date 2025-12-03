namespace FreeLink.Application.UseCase.Admin.Commands.RejectWithdrawal;

public class RejectWithdrawalResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? WithdrawalId { get; set; }
    public string? Status { get; set; }
}
