using Microsoft.AspNetCore.Mvc;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/payments/webhooks")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IUseCaseHandlePaymentNotification _handlePaymentNotification;

    public PaymentWebhookController(IUseCaseHandlePaymentNotification handlePaymentNotification)
    {
        _handlePaymentNotification = handlePaymentNotification;
    }

    [HttpPost]
    public IActionResult Receive([FromBody] PaymentNotificationDto request)
    {
        var result = _handlePaymentNotification.Execute(request);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return Ok();
    }
}
