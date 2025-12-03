using System.Diagnostics;
using System.Security.Claims;

namespace FreeLink.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Obtener información del usuario si está autenticado
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
        var userRole = context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "None";

        // Información de la petición
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.ToString();
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _logger.LogInformation(
            "[{RequestId}] Iniciando petición: {Method} {Path}{QueryString} | Usuario: {UserId} ({UserRole}) | IP: {IpAddress}",
            requestId, method, path, queryString, userId, userRole, ipAddress);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            var logLevel = statusCode >= 500 ? LogLevel.Error
                         : statusCode >= 400 ? LogLevel.Warning
                         : LogLevel.Information;

            _logger.Log(logLevel,
                "[{RequestId}] Petición completada: {Method} {Path} | Status: {StatusCode} | Tiempo: {ElapsedMs}ms | Usuario: {UserId}",
                requestId, method, path, statusCode, elapsedMs, userId);

            // Log de alertas para peticiones lentas (> 5 segundos)
            if (elapsedMs > 5000)
            {
                _logger.LogWarning(
                    "[{RequestId}] ⚠️ PETICIÓN LENTA detectada: {Method} {Path} | Tiempo: {ElapsedMs}ms",
                    requestId, method, path, elapsedMs);
            }
        }
    }
}
