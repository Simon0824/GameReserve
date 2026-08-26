using Games.Application;
using Games.Infrastructure;
using Identity.Application;
using Identity.Infrastructure;
using Reservations.Application;
using Reservations.Infrastructure;

namespace GameReserve.WebApi.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddApiDI(this IServiceCollection services)
    {
        services.AddIdentityApplicationDI()
                .AddIdentityIfrastructureDI();
        
        services.AddReservationsApplicationDI()
                .AddReservationsIfrastructureDI();
        
        services.AddGamesApplicationDI()
                .AddGamesInfrastructureDI();
        return services;
    }
}