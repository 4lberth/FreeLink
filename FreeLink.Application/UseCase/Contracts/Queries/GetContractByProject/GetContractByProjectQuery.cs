using MediatR;

namespace FreeLink.Application.UseCase.Contracts.Queries.GetContractByProject;

public class GetContractByProjectQuery : IRequest<GetContractByProjectResponse>
{
    public int ProjectId { get; set; }
    public int? RequestingUserId { get; set; }
}
