using FluentValidation;
using FluentValidation.AspNetCore;
using GameReserve.WebApi.BackgroundServices;
using GameReserve.WebApi.DependencyInjection;
using GameReserve.WebApi.Exceptions;
using GameReserve.WebApi.Extensions;
using Games.Infrastructure.Data;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Reservations.Application.Events;
using Reservations.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();

builder.Services.AddSwaggerExtension();

builder.Services.AddApiDI(builder.Configuration);

builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();

    busConfiguration.AddConsumer<UserCreatedEventConsumer>();

    busConfiguration.UsingInMemory((context, configurator) =>
    {
        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddProblemDetails(configuration =>
{   
    configuration.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Add("reqId", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHostedService<AdminSeedingService>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var scope = app.Services.CreateScope();
    var IdentityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    IdentityContext.Database.Migrate();

    var GamesContext = scope.ServiceProvider.GetRequiredService<GamesContext>();
    GamesContext.Database.Migrate();

    var ReservationsContext = scope.ServiceProvider.GetRequiredService<ReservationsContext>();
    ReservationsContext.Database.Migrate();
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