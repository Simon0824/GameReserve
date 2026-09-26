using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace GameReserve.WebApi.Controllers;
[ApiController]
[Route("webhook/stripe")]
public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendPayment()
    {
        try
        {
            return Ok();
        }
        catch(StripeException)
        {
            return BadRequest();
        }
    }
}