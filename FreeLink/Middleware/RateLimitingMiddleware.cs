using System.Collections.Concurrent;
using System.Net;

namespace FreeLink.Middleware;

/// <summary>
/// Middleware que implementa rate limiting basado en IP para prevenir abuso de la API
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, ClientRequestInfo> _clients = new();
    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        int requestLimit = 100,
        int timeWindowSeconds = 60)
    {
        _next = next;
        _logger = logger;
        _requestLimit = requestLimit;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Obtener o crear información del cliente
        var clientInfo = _clients.GetOrAdd(ipAddress, _ => new ClientRequestInfo
        {
            RequestCount = 0,
            WindowStart = DateTime.UtcNow
        });

        bool rateLimitExceeded = false;
        double retryAfter = 0;

        lock (clientInfo)
        {
            // Si la ventana de tiempo ha expirado, resetear contador
            if (DateTime.UtcNow - clientInfo.WindowStart > _timeWindow)
            {
                clientInfo.RequestCount = 0;
                clientInfo.WindowStart = DateTime.UtcNow;
            }

            // Incrementar contador
            clientInfo.RequestCount++;

            // Verificar si se excedió el límite
            if (clientInfo.RequestCount > _requestLimit)
            {
                rateLimitExceeded = true;
                retryAfter = (_timeWindow - (DateTime.UtcNow - clientInfo.WindowStart)).TotalSeconds;
            }
        }

        if (rateLimitExceeded)
        {
            _logger.LogWarning(
                "Rate limit excedido para IP: {IpAddress} | Path: {Path}",
                ipAddress, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            context.Response.Headers.Append("Retry-After", ((int)retryAfter).ToString());

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Demasiadas peticiones. Por favor, intenta más tarde.",
                errorCode = "RATE_LIMIT_EXCEEDED",
                retryAfter = $"{(int)retryAfter} segundos"
            });

            return;
        }

        await _next(context);
    }

    // Método para limpiar clientes inactivos periódicamente
    public static void CleanupInactiveClients(TimeSpan inactivityThreshold)
    {
        var now = DateTime.UtcNow;
        var keysToRemove = _clients
            .Where(kvp => now - kvp.Value.WindowStart > inactivityThreshold)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _clients.TryRemove(key, out _);
        }
    }
}

/// <summary>
/// Información de peticiones por cliente
/// </summary>
public class ClientRequestInfo
{
    public int RequestCount { get; set; }
    public DateTime WindowStart { get; set; }
}
