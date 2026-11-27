using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedKernel.Domain.Enums;
using SharedKernel.DTOs;

namespace Payments.Infrastructure.Nbp;
public class NbpExchangeRateClient(IDistributedCache cache, 
                                   HttpClient client, 
                                   ILogger<NbpExchangeRateClient> logger) : INbpRateClient
{
    private static string CurrencyKey(string currency) => $"exchange{currency}";

    public async Task SetExchange(CancellationToken cancellationToken)
    {
        foreach(string currency in Enum.GetNames(typeof(PaymentCurrency)))
        {
            if(currency != PaymentCurrency.none.ToString())
            {
                var response = await client.GetAsync($"api/exchangerates/rates/a/{currency}/?format=json", cancellationToken);
                
                response.EnsureSuccessStatusCode();


                var json = await response.Content.ReadFromJsonAsync<NbpExchangeResponseDTO>();

                Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

                var exchangeRate = json!.rates.First().mid;

                await cache.SetStringAsync(CurrencyKey(currency), exchangeRate.ToString(), cancellationToken);
            }
        }
    }
    public async Task<decimal> GetCurrencyExchange(PaymentCurrency currency, decimal amount, CancellationToken cancellationToken)
    {
        var exchange = await cache.GetStringAsync(CurrencyKey(currency.ToString()), cancellationToken);

        decimal.TryParse(exchange, out decimal exchangeParsed);

        var conversion = Math.Round((amount*exchangeParsed), 2, MidpointRounding.AwayFromZero);

        logger.LogInformation($"Amount after exchange to pln: {exchange}");

        return conversion;
    }
}