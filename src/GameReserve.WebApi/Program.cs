using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using GameReserve.WebApi.BackgroundServices;
using GameReserve.WebApi.DependencyInjection;
using GameReserve.WebApi.Exceptions;
using GameReserve.WebApi.Extensions;
using GameReserve.WebApi.Outbox;
using Games.Infrastructure.Data;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payments.Application.EventConsumers;
using Payments.Infrastructure.Data;
using Reservations.Application.EventConsumers;
using Reservations.Application.Events;
using Reservations.Infrastructure.Data;

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

builder.Services.AddDistributedRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Port"];
});

builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();

    busConfiguration.AddConsumer<UserCreatedEventConsumer>();

    busConfiguration.AddConsumer<ReservationCreatedEventConsumer>();

    busConfiguration.AddConsumer<PaymentConfirmedEventConsumer>();
    busConfiguration.AddConsumer<PaymentFailedEventConsumer>();

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

builder.Services.AddScoped(typeof(OutboxProcessor<>));
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHostedService<AdminSeedingService>();
builder.Services.AddHostedService<OutboxBackgroundService>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var scope = app.Services.CreateScope();
    var IdentityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    //IdentityContext.Database.Migrate();

    var GamesContext = scope.ServiceProvider.GetRequiredService<GamesContext>();
    //GamesContext.Database.Migrate();

    var ReservationsContext = scope.ServiceProvider.GetRequiredService<ReservationsContext>();
    //ReservationsContext.Database.Migrate();

    var PaymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
    //PaymentsContext.Database.Migrate();
}
else
{
    app.UseHttpsRedirection();
}

app.UseHostFiltering();

app.UseExceptionHandler();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();