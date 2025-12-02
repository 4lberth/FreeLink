using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Commands.SubmitDeliverable;

public class SubmitDeliverableCommand : IRequest<SubmitDeliverableResponse>
{
    public int DeliverableId { get; set; }
    public int FreelancerId { get; set; }
}
