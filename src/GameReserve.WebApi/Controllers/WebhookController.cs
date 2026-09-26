using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Commands;
using Stripe;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("webhook/stripe")]
public class WebhookController(IConfiguration configuration, ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendPayment()
    {
        Console.WriteLine($"Webhook secret exists: {!string.IsNullOrEmpty(configuration["Stripe:Webhook"])}");
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                configuration["Stripe:Webhook"]
            );

            switch(stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                var succeededPayment = stripeEvent.Data.Object as PaymentIntent;
                await sender.Send(new ConfirmPaymentCommand(succeededPayment!.Id));
                break;
                case "payment_intent.payment_failed":
                var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                await sender.Send(new FailedPaymentCommand(failedPayment!.Id));
                break;
            }
            return Ok();
        }
        catch(StripeException)
        {
            return BadRequest();
        }
    }
}