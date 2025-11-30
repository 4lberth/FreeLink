using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using FreeLink.Infrastructure.Adapters;
using FreeLink.Infrastructure.Data.Context;
using FreeLink.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FreeLink.Infrastructure.Configuration;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Registrar DbContext con MySQL
        services.AddDbContext<FreeLinkContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        
        // Registrar patrón Repository y UnitOfWork
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Registrar servicios
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(); 
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}