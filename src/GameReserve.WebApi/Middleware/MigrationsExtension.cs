using Games.Infrastructure.Data;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Payments.Infrastructure.Data;
using Reservations.Infrastructure.Data;

namespace GameReserve.WebApi.Middleware;
public static class MigrationsExtension
{
    public static async Task<IApplicationBuilder> ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        await using var scope =  app.ApplicationServices.CreateAsyncScope();

        var IdentityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
        await IdentityContext.Database.MigrateAsync();

        var GamesContext = scope.ServiceProvider.GetRequiredService<GamesContext>();
        await GamesContext.Database.MigrateAsync();

        var ReservationsContext = scope.ServiceProvider.GetRequiredService<ReservationsContext>();
        await ReservationsContext.Database.MigrateAsync();

        var PaymentsContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        await PaymentsContext.Database.MigrateAsync();

        return app;
    }
}