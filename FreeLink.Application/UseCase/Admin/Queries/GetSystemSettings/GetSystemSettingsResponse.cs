namespace FreeLink.Application.UseCase.Admin.Queries.GetSystemSettings;

public class GetSystemSettingsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<SystemSettingDto> Settings { get; set; } = new();
}

public class SystemSettingDto
{
    public int SettingId { get; set; }
    public string? SettingKey { get; set; }
    public string? SettingValue { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
}
