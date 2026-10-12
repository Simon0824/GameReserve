using GameReserve.WebApi.Outbox;
using Identity.Infrastructure.Data;
using Payments.Infrastructure.Data;
using Reservations.Infrastructure.Data;

namespace GameReserve.WebApi.BackgroundServices;
public class OutboxBackgroundService(ILogger<OutboxBackgroundService> logger, 
                                     IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const int Frequency = 6;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateAsyncScope();
                var outboxIdentity = scope.ServiceProvider.GetRequiredService<OutboxProcessor<IdentityContext>>();
                var outboxPayments = scope.ServiceProvider.GetRequiredService<OutboxProcessor<PaymentsContext>>();
                var outboxReservations = scope.ServiceProvider.GetRequiredService<OutboxProcessor<ReservationsContext>>();

                await outboxIdentity.ExecuteAsync(stoppingToken);
                await outboxPayments.ExecuteAsync(stoppingToken);
                await outboxReservations.ExecuteAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(Frequency));
            }
        }
        catch(OperationCanceledException)
        {
            logger.LogInformation("Operation canceled");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occured");
        }
        finally
        {
            logger.LogInformation("Outbox processed");
        }
    }
}