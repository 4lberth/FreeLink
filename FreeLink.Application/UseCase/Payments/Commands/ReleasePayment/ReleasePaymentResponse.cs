namespace FreeLink.Application.UseCase.Payments.Commands.ReleasePayment;

public class ReleasePaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public decimal? FreelancerAmount { get; set; }
    public decimal? CommissionAmount { get; set; }
    public string? ReceiptUrl { get; set; }
}
