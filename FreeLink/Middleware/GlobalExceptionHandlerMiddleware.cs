using System.Net;
using System.Text.Json;

namespace FreeLink.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no manejada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Success = false,
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "No autorizado";
                response.ErrorCode = "UNAUTHORIZED";
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Recurso no encontrado";
                response.ErrorCode = "NOT_FOUND";
                break;

            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                response.ErrorCode = "BAD_REQUEST";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = _environment.IsDevelopment()
                    ? exception.Message
                    : "Ha ocurrido un error interno. Por favor, contacta al soporte.";
                response.ErrorCode = "INTERNAL_ERROR";

                // Solo en desarrollo, incluir detalles técnicos
                if (_environment.IsDevelopment())
                {
                    response.Details = new
                    {
                        exception.StackTrace,
                        InnerException = exception.InnerException?.Message
                    };
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public object? Details { get; set; }
}
