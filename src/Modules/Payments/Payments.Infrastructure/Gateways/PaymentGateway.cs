using Microsoft.Extensions.Configuration;
using Payments.Domain.Iterfaces;
using Payments.Domain.Primitives;
using Stripe;

namespace Payments.Infrastructure.Gateways;
public class StripePaymentGateway(IConfiguration configuration) : IPaymentGateway
{
    private readonly IStripeClient _stripeClient = new StripeClient(configuration["Stripe:ApiKey"]);
    public async Task<string> CreatePaymentIntentAsync(ReservationId reservationId, decimal amount)
    {

        var options = new PaymentIntentCreateOptions()
        {
            Amount = (long)(amount * 100),
            Currency = "usd",
            Metadata = new Dictionary<string, string>
            {
                {"reservationid", reservationId.Value.ToString()}
            },
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            },
            Confirm = true,
            PaymentMethod = "pm_visa_card"
        };

        var service = new PaymentIntentService(_stripeClient);

        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.Id;
    }
}