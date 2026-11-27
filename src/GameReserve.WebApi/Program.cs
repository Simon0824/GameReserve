using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using GameReserve.WebApi.BackgroundServices;
using GameReserve.WebApi.DependencyInjection;
using GameReserve.WebApi.Exceptions;
using GameReserve.WebApi.Extensions;
using GameReserve.WebApi.Health;
using GameReserve.WebApi.Middleware;
using GameReserve.WebApi.Outbox;
using Games.Infrastructure.Data;
using GemeReserve.WebApi.Inbox;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Payments.Application.PaymentsInboxConsumer;
using Payments.Infrastructure.Data;
using Payments.Infrastructure.Nbp;
using Reservations.Application.InboxConsumer;
using Reservations.Infrastructure.Data;
using SharedKernel.IntegrationEvents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.ConfigureHttpJsonOptions(options =>
{
   options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter()
                    );
                }
                );

builder.Services.AddSwaggerExtension();

builder.Services.AddApiDI(builder.Configuration);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "GameReserve";
});

builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();

    busConfiguration.AddConsumer<ReservationsInboxConsumer<UserCreatedIntegrationEvent>>();
    busConfiguration.AddConsumer<ReservationsInboxConsumer<PaymentConfirmedIntegrationEvent>>();
    busConfiguration.AddConsumer<ReservationsInboxConsumer<PaymentFailedIntegrationEvent>>();

    busConfiguration.AddConsumer<PaymentsInboxConsumer<ReservationCreatedIntegrationEvent>>();

    busConfiguration.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddProblemDetails(configuration =>
{   
    configuration.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Add("reqId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("Default")!);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped(typeof(OutboxProcessor<>));
builder.Services.AddScoped(typeof(InboxProcessor<>));
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHostedService<AdminSeedingService>();
builder.Services.AddHostedService<OutboxBackgroundService>();
builder.Services.AddHostedService<InboxBackgroundService>();

builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("custom-healthcheck", HealthStatus.Unhealthy)
            .AddNpgSql(builder.Configuration.GetConnectionString("Default")!)
            .AddRedis(builder.Configuration.GetConnectionString("Redis")!)
            .AddDbContextCheck<IdentityContext>()
            .AddDbContextCheck<ReservationsContext>()
            .AddDbContextCheck<GamesContext>()
            .AddDbContextCheck<PaymentsContext>();

builder.Services.AddHttpClient("NbpClient", client =>
{
    client.BaseAddress = new Uri("https://api.nbp.pl/");
});

builder.Services.AddHostedService<GetExchangeRate>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrationsAsync();
}
else
{
    app.UseHttpsRedirection();
}

app.MapGet("/health", async(HealthCheckService healthcheck) =>
{
    var check = await healthcheck.CheckHealthAsync(); 

    return check.Status == HealthStatus.Healthy 
                                ? Results.Ok(check) 
                                : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
})
.WithTags("Health");

app.UseHostFiltering();

app.UseExceptionHandler();

app.UseRouting();

app.UseAuthentication();
app.UseCheckIsUserBanned();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}