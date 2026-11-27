using Payments.Infrastructure.Nbp;

namespace GameReserve.WebApi.BackgroundServices;
public class SetExchangeRate(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var nbpRate = scope.ServiceProvider.GetRequiredService<NbpExchangeRateClient>();

        await nbpRate.SetExchange(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}