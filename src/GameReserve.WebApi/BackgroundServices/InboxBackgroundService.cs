using GemeReserve.WebApi.Inbox;
using Payments.Infrastructure.Data;
using Reservations.Infrastructure.Data;

namespace GameReserve.WebApi.BackgroundServices;
public class InboxBackgroundService(ILogger<InboxBackgroundService> logger, 
                                     IServiceScopeFactory scopeFactory) : BackgroundService
{
    private const int Frequency = 7500;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        try
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateAsyncScope();
                var inboxReservations = scope.ServiceProvider.GetRequiredService<InboxProcessor<ReservationsContext>>();
                var inboxPayments = scope.ServiceProvider.GetRequiredService<InboxProcessor<PaymentsContext>>();

                await Task.WhenAll(
                inboxReservations.ExecuteAsync(stoppingToken),
                inboxPayments.ExecuteAsync(stoppingToken)
                );

                await Task.Delay(TimeSpan.FromMilliseconds(Frequency));
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
            logger.LogInformation("Inbox processed");
        }
    }
}