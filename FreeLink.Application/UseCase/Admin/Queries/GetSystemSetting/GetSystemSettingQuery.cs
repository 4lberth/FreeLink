using MediatR;

namespace FreeLink.Application.UseCase.Admin.Queries.GetSystemSetting;

public class GetSystemSettingQuery : IRequest<GetSystemSettingResponse>
{
    public string SettingKey { get; set; } = string.Empty;
    public int RequestingAdminId { get; set; }
}
