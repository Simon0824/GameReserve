using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Domain.Interfaces;
using Payments.Domain.Iterfaces;
using Payments.Infrastructure.Data;
using Payments.Infrastructure.Gateways;
using Payments.Infrastructure.Repositories;

namespace Payments.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPaymentsInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"));
        });

        services.AddSingleton<IPaymentGateway, StripePaymentGateway>();
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        return services;
    }
}
