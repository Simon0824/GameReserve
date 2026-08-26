using Microsoft.Extensions.DependencyInjection;

namespace Reservations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsApplicationDI(this IServiceCollection services)
    {
        return services;
    }
}
