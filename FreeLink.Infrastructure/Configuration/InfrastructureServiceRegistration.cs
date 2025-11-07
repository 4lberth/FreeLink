using FreeLink.Infrastructure.Data.Context;
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

        // // Registrar patrón Repository y UnitOfWork
        // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        //
        // // Registrar el generador de tokens JWT
        // services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}