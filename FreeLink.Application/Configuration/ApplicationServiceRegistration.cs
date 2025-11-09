// csharp
using Microsoft.Extensions.DependencyInjection;
using FreeLink.Application.Services;

namespace FreeLink.Application.Configuration
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar servicios de la capa Application
            services.AddSingleton<IProposalService, InMemoryProposalService>();

            // Añadir otros registros existentes si los hay...
            return services;
        }
    }
}