using Identity.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.Constants;

namespace GameReserve.WebApi.BackgroundServices;
public class AdminSeedingService(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
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

        var adminEmail = configuration["Admin:Email"] 
                ?? throw new InvalidOperationException("Admin:Email is not configured.");

        var adminFullName = configuration["Admin:FullName"]
                ?? throw new InvalidOperationException("Admin:FullName is not configured.");

        var adminPassword = configuration["Admin:Password"]
                ?? throw new InvalidOperationException("Admin:Password is not configured.");

        if(await userManager.FindByEmailAsync(adminEmail!) is null)
        {
            var admin = User.Create(adminFullName, adminEmail);

            var createResult = await userManager.CreateAsync(admin, adminPassword!);

        if(!createResult.Succeeded)
        { 
            Console.WriteLine(string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(admin, UserRoles.Admin);

        if(!roleResult.Succeeded)
        {
            Console.WriteLine(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}