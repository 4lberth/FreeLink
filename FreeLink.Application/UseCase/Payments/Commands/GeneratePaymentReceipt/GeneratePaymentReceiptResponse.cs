namespace FreeLink.Application.UseCase.Payments.Commands.GeneratePaymentReceipt;

public class GeneratePaymentReceiptResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? ReceiptUrl { get; set; }
    public byte[]? PdfBytes { get; set; }
}
