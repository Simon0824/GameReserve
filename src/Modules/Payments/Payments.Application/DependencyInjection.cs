using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Abstractions;

namespace Payments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        return services;
    }
}
