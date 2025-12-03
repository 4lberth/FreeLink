using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetTicketDetails;

public class GetTicketDetailsQuery : IRequest<GetTicketDetailsResponse>
{
    public int TicketId { get; set; }
    public int RequestingAdminId { get; set; }
}
