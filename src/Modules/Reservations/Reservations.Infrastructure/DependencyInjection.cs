using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Infrastructure.Data;

namespace Reservations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReservationsContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
        });
        return services;
    }
}
