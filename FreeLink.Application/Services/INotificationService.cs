using System.Threading.Tasks;

namespace FreeLink.Application.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string message);
    }
}