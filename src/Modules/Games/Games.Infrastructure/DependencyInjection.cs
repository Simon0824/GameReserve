using Games.Domain.Interfaces;
using Games.Infrastructure.Data;
using Games.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGamesInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GamesContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<IGameRepository, GameRepository>();
        return services;
    }
}
