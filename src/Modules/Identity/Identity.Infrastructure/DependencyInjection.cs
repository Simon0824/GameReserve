using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Default"));
        });
        return services;
    }
}
