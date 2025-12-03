using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingVerifications;

public class GetPendingVerificationsQuery : IRequest<GetPendingVerificationsResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
