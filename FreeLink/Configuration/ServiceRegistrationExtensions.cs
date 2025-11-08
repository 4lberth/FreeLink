using FreeLink.Application.Configuration;
using FreeLink.Infrastructure.Configuration;
using Microsoft.OpenApi.Models;

namespace FreeLink.Configuration;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        // Registrar servicios de infraestructura
        services.AddInfrastructureServices(configuration);
        // 1. Habilitar controladores
        services.AddControllers();
        // 2. Configuración de Swagger/OpenAPI
        services.AddEndpointsApiExplorer(); 
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "FreeLink API", 
                Version = "v1",
                Description = "API con Arquitectura Limpia y Hexagonal"
            });
            
            // Configuración para JWT en Swagger (para más adelante)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Ejemplo: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
    
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}