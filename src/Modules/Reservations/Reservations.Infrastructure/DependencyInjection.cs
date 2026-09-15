using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservations.Domain.Interfaces;
using Reservations.Infrastructure.Data;
using Reservations.Infrastructure.Repositories;

namespace Reservations.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReservationsInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReservationsContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<IReservationsRepository, ReservationsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
