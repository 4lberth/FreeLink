using MediatR;

namespace FreeLink.Application.UseCase.ActivityLogs.Queries.GetProjectActivityLog;

public class GetProjectActivityLogQuery : IRequest<GetProjectActivityLogResponse>
{
    public int ProjectId { get; set; }
    public int? RequestingUserId { get; set; }
}
