using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.ApproveDeliverable;

public class ApproveDeliverableCommand : IRequest<ApproveDeliverableResponse>
{
    public int DeliverableId { get; set; }
    public int ClientId { get; set; }
    public string? Comments { get; set; }
}
