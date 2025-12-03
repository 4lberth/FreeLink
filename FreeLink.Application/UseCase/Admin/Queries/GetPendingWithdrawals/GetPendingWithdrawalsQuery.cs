using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetPendingWithdrawals;

public class GetPendingWithdrawalsQuery : IRequest<GetPendingWithdrawalsResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
