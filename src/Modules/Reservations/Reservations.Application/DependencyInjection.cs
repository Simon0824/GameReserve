using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Reservations.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
