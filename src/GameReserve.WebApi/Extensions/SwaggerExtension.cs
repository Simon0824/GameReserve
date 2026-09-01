using Microsoft.OpenApi;

namespace GameReserve.WebApi.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddSwaggerGen( options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GameReserve",
                Version ="v1"
            });


            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
               Type = SecuritySchemeType.Http,
               Scheme = "Bearer",
               Description = "Enter token only" 
            });

            options.AddSecurityRequirement(document =>
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });
        });
        
        return services;
    }
}
