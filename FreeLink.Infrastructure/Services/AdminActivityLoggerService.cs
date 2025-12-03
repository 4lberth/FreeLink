using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;

namespace FreeLink.Infrastructure.Services;

public class AdminActivityLoggerService : IAdminActivityLogger
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminActivityLoggerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogActivity(int adminId, string actionType, string actionDetails, string? ipAddress = null)
    {
        var log = new Adminactivitylog
        {
            AdminId = adminId,
            ActionType = actionType,
            ActionDescription = actionDetails,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Adminactivitylog>().Add(log);
        await _unitOfWork.Complete();
    }
}