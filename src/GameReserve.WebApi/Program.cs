using FluentValidation;
using FluentValidation.AspNetCore;
using GameReserve.WebApi.DependencyInjection;
using GameReserve.WebApi.Exceptions;
using GameReserve.WebApi.Extensions;
using Games.Infrastructure.Data;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();

builder.Services.AddSwaggerExtension();

builder.Services.AddApiDI(builder.Configuration);

builder.Services.AddMassTransit(busConfiguration =>
{
    busConfiguration.SetKebabCaseEndpointNameFormatter();

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

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    if(!await roleManager.RoleExistsAsync(UserRoles.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
    }

    if(!await roleManager.RoleExistsAsync(UserRoles.User))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
    }

    var adminEmail = app.Configuration["Admin:Email"];
    var adminFullName = app.Configuration["Admin:FullName"];
    var adminPassowrd = app.Configuration["Admin:Password"];

    if(await userManager.FindByEmailAsync(adminEmail!) is null)
    {
    var admin = User.Create(adminFullName!, adminEmail!);

    var createResult = await userManager.CreateAsync(admin, adminPassowrd!);

    if(!createResult.Succeeded)
    {
        Console.WriteLine(string.Join(", ", createResult.Errors.Select(e => e.Description)));
    }

    var roleResult = await userManager.AddToRoleAsync(admin, UserRoles.Admin);

    if(!roleResult.Succeeded)
    {
        Console.WriteLine(string.Join(", ", createResult.Errors.Select(e => e.Description)));
    }
    }
}
else
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();