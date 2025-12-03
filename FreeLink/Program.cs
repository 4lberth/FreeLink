using FreeLink.Configuration;
using FreeLink.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios ---
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// --- Configuración de Middlewares (orden importante) ---

// 1. Global Exception Handler - Debe ser el primero para capturar todas las excepciones
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// 2. Request Logging - Log de todas las peticiones
app.UseMiddleware<RequestLoggingMiddleware>();

// 3. Swagger - Solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeLink API v1");
    });
}

// 4. CORS - Antes de autenticación
app.UseCors("AllowSpecificOrigins");

// 5. Rate Limiting - Limitar peticiones antes de operaciones costosas
// Nota: Configurar límites según necesidades (100 peticiones por minuto por defecto)
app.UseMiddleware<RateLimitingMiddleware>();

// 6. Security Headers - Headers de seguridad HTTP
app.UseMiddleware<SecurityHeadersMiddleware>();

// 7. HTTPS Redirection
app.UseHttpsRedirection();

// 8. Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// 9. Mapeo de controladores
app.MapControllers();

app.Run();