using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllSanctions;

public class GetAllSanctionsQuery : IRequest<GetAllSanctionsResponse>
{
    public int RequestingAdminId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SanctionType { get; set; } // Filter by type
    public bool? IsActive { get; set; } // Filter by active status
}
