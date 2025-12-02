using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.RejectDeliverable;

public class RejectDeliverableCommand : IRequest<RejectDeliverableResponse>
{
    public int DeliverableId { get; set; }
    public int ClientId { get; set; }
    public string RejectionComments { get; set; } = null!; // Obligatorio
}
