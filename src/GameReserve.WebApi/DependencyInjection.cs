using Games.Application;
using Games.Infrastructure;
using Identity.Application;
using Identity.Infrastructure;
using Payments.Application;
using Payments.Infrastructure;
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
                .AddReservationsInfrastructureDI(cfg);
        
        services.AddGamesApplicationDI()
                .AddGamesInfrastructureDI(cfg);

        services.AddPaymentsApplicationDI()
                .AddPaymentsInfrastructureDI(cfg);
        
        return services;
    }
}