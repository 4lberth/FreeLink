using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetDisputeDetails;

public class GetDisputeDetailsQuery : IRequest<GetDisputeDetailsResponse>
{
    public int DisputeId { get; set; }
    public int RequestingAdminId { get; set; }
}
