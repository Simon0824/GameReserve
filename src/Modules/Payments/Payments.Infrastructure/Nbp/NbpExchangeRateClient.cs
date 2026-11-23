using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Payments.Domain.Enums;

namespace Payments.Infrastructure.Nbp;
public class NbpExchangeRateClient(HttpClient httpClient, ILogger<NbpExchangeRateClient> logger) : INbpRateClient
{
    public async Task<decimal> GetCurrencyExchange(PaymentCurrency currency, decimal amount, CancellationToken cancellationToken)
    {
        var url = $"api/exchangerates/rates/a/{currency}/?format=json";

        Console.WriteLine($"BaseAddress: {httpClient.BaseAddress}");
        Console.WriteLine($"Currency: {currency}");
        Console.WriteLine($"Final URL: {new Uri(httpClient.BaseAddress!, url)}");

        var response = await httpClient.GetAsync(
                                                $"api/exchangerates/rates/a/{currency.ToString()}/?format=json", cancellationToken);
        
        response.EnsureSuccessStatusCode();


        var json = await response.Content.ReadFromJsonAsync<NbpExchangeResponseDTO>();

        Console.WriteLine(JsonSerializer.Serialize(json, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        
        var exchange = Math.Round(json!.rates.First().mid * amount, 2, MidpointRounding.AwayFromZero);

        logger.LogInformation($"Amount after exchange to pln: {exchange}");

        return exchange;
    }
}