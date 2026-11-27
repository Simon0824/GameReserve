using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Payments.Domain.Aggregates;
using Payments.Infrastructure.Nbp;
using SharedKernel.Domain.Enums;

namespace GameReserve.WebApi.BackgroundServices;
public class GetExchangeRate(IDistributedCache cache, IHttpClientFactory httpFactory) : IHostedService
{
    private static string CurrencyKey(string currency) => $"exchange{currency}";
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var httpClient = httpFactory.CreateClient("NbpClient");

        foreach(string currency in Enum.GetNames(typeof(PaymentCurrency)))
        {
            if(currency != PaymentCurrency.none.ToString())
            {
                var response = await httpClient.GetAsync($"api/exchangerates/rates/a/{currency}/?format=json",  cancellationToken);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadFromJsonAsync<NbpExchangeResponseDTO>();

                Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions()
                {
                    WriteIndented = true
                }));

                var exchange = json!.rates.First().mid;

                await cache.SetStringAsync(CurrencyKey(currency), exchange.ToString(), cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}