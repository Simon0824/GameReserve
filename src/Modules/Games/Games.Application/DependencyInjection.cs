using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Abstractions.Behaviors;

namespace Games.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGamesApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        return services;
    }
}
