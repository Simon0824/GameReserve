using Microsoft.Extensions.DependencyInjection;

namespace Payments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsApplicationDI(this IServiceCollection services)
    {
        return services;
    }
}
