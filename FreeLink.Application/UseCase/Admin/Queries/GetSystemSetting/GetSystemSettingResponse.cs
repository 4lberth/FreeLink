namespace FreeLink.Application.UseCase.Admin.Queries.GetSystemSetting;

public class GetSystemSettingResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public SystemSettingDetailsDto? Data { get; set; }
}

public class SystemSettingDetailsDto
{
    public int SettingId { get; set; }
    public string? SettingKey { get; set; }
    public string? SettingValue { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
}
