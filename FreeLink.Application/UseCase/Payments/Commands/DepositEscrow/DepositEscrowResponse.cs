namespace FreeLink.Application.UseCase.Payments.Commands.DepositEscrow;

public class DepositEscrowResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public int? EscrowId { get; set; }
    public decimal? Amount { get; set; }
}
