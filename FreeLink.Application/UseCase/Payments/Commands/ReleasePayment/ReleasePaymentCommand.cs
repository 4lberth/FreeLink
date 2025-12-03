using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.ReleasePayment;

public class ReleasePaymentCommand : IRequest<ReleasePaymentResponse>
{
    public int DeliverableId { get; set; }
    public int ClientId { get; set; }
}
