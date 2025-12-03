using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetUserSanctions;

public class GetUserSanctionsQuery : IRequest<GetUserSanctionsResponse>
{
    public int UserId { get; set; }
    public int RequestingAdminId { get; set; }
}
