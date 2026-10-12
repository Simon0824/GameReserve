using SharedKernel.Application.Abstractions.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Identy.Application.IdentityAppliactionMarker;

namespace Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(IdentityApplicationMarker).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        return services;
    }
}
