using FreeLink.Application.UseCase.Projects.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectApplications;

public class GetProjectApplicationsQuery : IRequest<GetProjectApplicationsResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
}
