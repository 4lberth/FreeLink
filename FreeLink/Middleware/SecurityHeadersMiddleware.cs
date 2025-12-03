namespace FreeLink.Middleware;

/// <summary>
/// Middleware que agrega headers de seguridad HTTP para proteger contra ataques comunes
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // X-Content-Type-Options: Previene MIME type sniffing
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // X-Frame-Options: Protege contra clickjacking
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // X-XSS-Protection: Protección XSS para navegadores antiguos
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Referrer-Policy: Controla información del referrer
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Content-Security-Policy: Previene XSS y otros ataques de inyección
        // Nota: Ajusta esta política según tus necesidades específicas
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data:; " +
            "connect-src 'self'");

        // Permissions-Policy: Controla features del navegador
        context.Response.Headers.Append("Permissions-Policy",
            "geolocation=(), microphone=(), camera=()");

        // Strict-Transport-Security: Fuerza HTTPS (solo en producción)
        if (!context.Request.Host.Host.Contains("localhost"))
        {
            context.Response.Headers.Append("Strict-Transport-Security",
                "max-age=31536000; includeSubDomains");
        }

        await _next(context);
    }
}
