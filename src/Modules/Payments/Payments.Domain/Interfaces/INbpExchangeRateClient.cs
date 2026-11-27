using SharedKernel.Domain.Enums;

namespace Payments.Infrastructure.Nbp;
public interface INbpRateClient
{
    Task<decimal> GetCurrencyExchange(PaymentCurrency currency, decimal amount, CancellationToken cancellationToken);
}