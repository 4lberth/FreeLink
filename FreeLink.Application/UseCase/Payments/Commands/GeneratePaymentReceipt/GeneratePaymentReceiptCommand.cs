using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.GeneratePaymentReceipt;

public class GeneratePaymentReceiptCommand : IRequest<GeneratePaymentReceiptResponse>
{
    public int TransactionId { get; set; }
    public int RequestingUserId { get; set; }
}
