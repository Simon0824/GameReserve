using Identity.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Iterfaces;
using Payments.Domain.Interfaces;
using Payments.Infrastructure.Behaviors;
using Payments.Infrastructure.Data;
using Payments.Infrastructure.Gateways;
using Payments.Infrastructure.Nbp;
using Payments.Infrastructure.Processors;
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

        var paymentProvider = configuration["Payment:Provider"];

        switch(paymentProvider)
        {
          case "Fake":
            services.AddScoped<IPaymentGateway, FakePaymentGateway>();
          break;
          case "Stripe":
            services.AddSingleton<IPaymentGateway, StripePaymentGateway>();
          break;
          default:
            throw new Exception("Payment provider not configured!");  
        };
        
        services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        services.AddScoped<IPaymentsOutboxRepository, PaymentsOutboxRepository>();
        services.AddScoped<IPaymentsInboxRepository, PaymentsInboxRepository>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PaymentsTransactionalBehavior<,>));
        services.AddScoped<FakePaymentProcessor>();
        services.AddScoped<INbpRateClient, NbpExchangeRateClient>();
        
        return services;
    }
}
