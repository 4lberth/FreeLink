using FreeLink.Application.UseCase.Projects.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetClientProjects;

public class GetClientProjectsQuery : IRequest<GetClientProjectsResponse>
{
    public int ClientId { get; set; }
    public string? StatusFilter { get; set; }
}
