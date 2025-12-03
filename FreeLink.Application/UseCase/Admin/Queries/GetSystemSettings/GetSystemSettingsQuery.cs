using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetSystemSettings;

public class GetSystemSettingsQuery : IRequest<GetSystemSettingsResponse>
{
    public int RequestingAdminId { get; set; }
}
