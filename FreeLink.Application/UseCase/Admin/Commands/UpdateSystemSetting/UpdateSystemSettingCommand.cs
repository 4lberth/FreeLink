using MediatR;

namespace FreeLink.Application.UseCase.Admin.Commands.UpdateSystemSetting;

public class UpdateSystemSettingCommand : IRequest<UpdateSystemSettingResponse>
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public int RequestingAdminId { get; set; }
}
