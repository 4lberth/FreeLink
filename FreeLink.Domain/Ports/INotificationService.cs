namespace FreeLink.Domain.Ports;

/// Servicio para gestionar notificaciones y logs de actividad del sistema
public interface INotificationService
{
    /// Registra una actividad en el log del proyecto
    Task LogProjectActivityAsync(int projectId, int? userId, string activityType, string description);

    /// Crea una notificación para un usuario
    Task CreateNotificationAsync(int userId, string type, string title, string message,
        string? resourceType = null, int? resourceId = null);

    /// Crea notificaciones para múltiples usuarios
    Task CreateMultipleNotificationsAsync(List<int> userIds, string type, string title, string message,
        string? resourceType = null, int? resourceId = null);
}
