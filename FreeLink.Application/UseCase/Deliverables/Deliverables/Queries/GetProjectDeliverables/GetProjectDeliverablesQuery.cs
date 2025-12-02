using MediatR;

namespace FreeLink.Application.UseCase.Deliverables.Queries.GetProjectDeliverables;

public class GetProjectDeliverablesQuery : IRequest<GetProjectDeliverablesResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
