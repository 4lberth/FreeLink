using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectDashboard;

public class GetProjectDashboardQuery : IRequest<GetProjectDashboardResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
