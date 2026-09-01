using System.Text;
using Identity.Domain.Interfaces;
using Identity.Domain.UserAggregate;
using Identity.Infrastructure.Auth;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("Default"));
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenProvider, TokenProvider>();        

        services.AddIdentity<User, IdentityRole>(options =>
        {
           options.Password.RequireUppercase = true;
           options.Password.RequiredLength = 6;
           options.Password.RequireDigit = true;

           options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<IdentityContext>();

        services.AddAuthorization()
                .AddAuthentication(option =>
                {
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
                {
                    config.TokenValidationParameters.ValidIssuer = configuration["Jwt:Issuer"];
                    config.TokenValidationParameters.ValidAudience = configuration["Jwt:Audience"];
                    config.TokenValidationParameters.IssuerSigningKey = 
                                                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
                });
        
        return services;
    }
}
