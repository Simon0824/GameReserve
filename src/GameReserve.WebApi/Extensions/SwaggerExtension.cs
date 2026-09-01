using Microsoft.OpenApi;

namespace GameReserve.WebApi.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddSwaggerGen( c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "GameReserve", Version ="v1"});


            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
               Type = SecuritySchemeType.Http,
               Scheme = "Bearer",
               Description = "Enter token only" 
            });
        }
        );
        return services;
    }
}
