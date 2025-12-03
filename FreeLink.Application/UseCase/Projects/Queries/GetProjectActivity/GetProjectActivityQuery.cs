using MediatR;

namespace FreeLink.Application.UseCase.Projects.Queries.GetProjectActivity;

public class GetProjectActivityQuery : IRequest<GetProjectActivityResponse>
{
    public int ProjectId { get; set; }
    public int RequestingUserId { get; set; }
    public int? Limit { get; set; } = 20;
}
