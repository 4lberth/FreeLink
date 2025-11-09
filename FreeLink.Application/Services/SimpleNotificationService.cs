using System;
using System.Threading.Tasks;
using FreeLink.Application.Services;

namespace FreeLink.Infrastructure.Services
{
    // Stub simple: escribe en consola. Reemplaza por SignalR / Email / FCM cuando quieras.
    public class SimpleNotificationService : INotificationService
    {
        public Task SendNotificationAsync(int userId, string message)
        {
            Console.WriteLine($"[Notify] user:{userId} - {message}");
            return Task.CompletedTask;
        }
    }
}