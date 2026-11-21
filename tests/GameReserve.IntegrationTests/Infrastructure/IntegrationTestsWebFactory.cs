using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Identity.Infrastructure.Data;
using Reservations.Infrastructure.Data;
using Games.Infrastructure.Data;
using Payments.Infrastructure.Data;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Redis;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Org.BouncyCastle.Asn1.X509.Qualified;
using GameReserve.WebApi.BackgroundServices;
using Microsoft.Extensions.Configuration;

namespace GameReserve.IntegrationTests.Infrastructure;
public class IntegrationTestsWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RedisContainer _redis = new RedisBuilder("redis:alpine").Build();

    private readonly RabbitMqContainer _rabbitMq = 
                                        new RabbitMqBuilder("rabbitmq:3-management")
                                        .WithUsername("guest")
                                        .WithPassword("guest")
                                        .Build();
    private readonly PostgreSqlContainer _postgreSql = 
                                        new PostgreSqlBuilder("postgres:latest")
                                        .WithDatabase("gamereserve_db")
                                        .WithUsername("postgres")
                                        .WithPassword("postgres")
                                        .Build();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
           var massTransitDescript = services.Where(s => s.ServiceType.FullName?.StartsWith("MassTransit") == true).ToList(); 

           foreach(var descriptor in massTransitDescript)
            {
                services.Remove(descriptor);
            }

            var rabbitMqDescript = services.SingleOrDefault(s => s.ServiceType.FullName?.StartsWith("RabbitMq") == true);

            if(rabbitMqDescript is not null)
            {
                services.Remove(rabbitMqDescript);
            } 

            var identityDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<IdentityContext>));

            if(identityDescriptor is not null)
            {
                services.Remove(identityDescriptor);
            }

            var reservationsDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ReservationsContext>));

            if(reservationsDescriptor is not null)
            {
                services.Remove(reservationsDescriptor);
            }

            var gamesDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<GamesContext>));

            if(gamesDescriptor is not null)
            {
                services.Remove(gamesDescriptor);
            }

            var paymentsDescriptor = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<PaymentsContext>));

            if(paymentsDescriptor is not null)
            {
                services.Remove(paymentsDescriptor);
            }

            var distributedCache = services.SingleOrDefault(s => s.ServiceType == typeof(IDistributedCache));

            if(distributedCache is not null)
            {
                services.Remove(distributedCache);
            }

            services.RemoveAll(typeof(IConnectionMultiplexer));


            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseNpgsql(_postgreSql.GetConnectionString());
            });

            services.AddDbContext<ReservationsContext>(options =>
            {
                options.UseNpgsql(_postgreSql.GetConnectionString());
            });

            services.AddDbContext<PaymentsContext>(options =>
            {
                options.UseNpgsql(_postgreSql.GetConnectionString());
            });

            services.AddDbContext<GamesContext>(options =>
            {
                options.UseNpgsql(_postgreSql.GetConnectionString());
            });


            services.AddDistributedMemoryCache();

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(new ConfigurationOptions()
            {
                EndPoints = {_redis.GetConnectionString()},
                AbortOnConnectFail = false
            }));

            services.AddMassTransitTestHarness(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(_rabbitMq.GetConnectionString(), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Admin:Email"] = "admin@example.com",
                ["Admin:Password"] = "TestPassword123!"
            });
        });
    }
    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgreSql.StartAsync(),
            _rabbitMq.StartAsync(),
            _redis.StartAsync()
        );

        using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IdentityContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<ReservationsContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<PaymentsContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<GamesContext>().Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await Task.WhenAll(
            _postgreSql.StopAsync(),
            _rabbitMq.StopAsync(),
            _redis.StopAsync()
        );

        await base.DisposeAsync();
    }
}