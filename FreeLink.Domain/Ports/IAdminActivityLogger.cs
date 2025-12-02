namespace FreeLink.Domain.Ports;

public interface IAdminActivityLogger
{
    Task LogActivity(int adminId, string actionType, string actionDetails, string? ipAddress = null);
}
