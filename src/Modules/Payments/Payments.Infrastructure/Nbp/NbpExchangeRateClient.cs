using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Payments.Domain.Enums;
using SharedKernel.Domain.Enums;

namespace Payments.Infrastructure.Nbp;
public class NbpExchangeRateClient(IDistributedCache cache, ILogger<NbpExchangeRateClient> logger) : INbpRateClient
{
    private static string CurrencyKey(string currency) => $"exchange{currency}";
    public async Task<decimal> GetCurrencyExchange(PaymentCurrency currency, decimal amount, CancellationToken cancellationToken)
    {
        var exchange = await cache.GetStringAsync(CurrencyKey(currency.ToString()), cancellationToken);

        decimal.TryParse(exchange, out decimal exchangeParsed);

        var conversion = Math.Round((amount*exchangeParsed), 2, MidpointRounding.AwayFromZero);

        logger.LogInformation($"Amount after exchange to pln: {exchange}");

        return conversion;
    }
}