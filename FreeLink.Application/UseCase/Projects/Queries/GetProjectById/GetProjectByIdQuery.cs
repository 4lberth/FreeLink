using FreeLink.Application.UseCase.Projects.DTOs;
using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectById;

public class GetProjectByIdQuery : IRequest<GetProjectByIdResponse>
{
    public int ProjectId { get; set; }
    public int? RequestingUserId { get; set; }
}
