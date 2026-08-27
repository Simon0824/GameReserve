using Games.Application;
using Games.Infrastructure;
using Identity.Application;
using Identity.Infrastructure;
using Reservations.Application;
using Reservations.Infrastructure;

namespace GameReserve.WebApi.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApiDI(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddIdentityApplicationDI()
                .AddIdentityInfrastructureDI(cfg);
        
        services.AddReservationsApplicationDI()
                .AddReservationsIfrastructureDI();
        
        services.AddGamesApplicationDI()
                .AddGamesInfrastructureDI();
        return services;
    }
}