using GameReserve.WebApi.DependencyInjection;
using GameReserve.WebApi.Extensions;
using Identity.Domain.Constants;
using Identity.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var scope = app.Services.CreateScope();
    var IdentityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    IdentityContext.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if(!await roleManager.RoleExistsAsync(UserRoles.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
    }

    if(!await roleManager.RoleExistsAsync(UserRoles.User))
    {
        await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
    }
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();