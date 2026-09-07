using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGamesApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
