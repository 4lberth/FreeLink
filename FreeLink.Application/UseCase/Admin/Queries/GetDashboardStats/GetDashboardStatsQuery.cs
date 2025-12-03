using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<GetDashboardStatsResponse>
{
    public int RequestingAdminId { get; set; }
}
