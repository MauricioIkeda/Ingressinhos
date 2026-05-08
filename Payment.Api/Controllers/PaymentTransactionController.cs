using Generic.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/payments/transactions")]
public class PaymentTransactionController : ApiQuery<PaymentTransaction>
{
    private readonly IUseCasePaymentTransactionCollection _useCaseCollection;

    public PaymentTransactionController(IUseCasePaymentTransactionCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return QueryAllResult();
    }

    [HttpPost]
    public IActionResult RequestPayment([FromBody] RequestPaymentDto request)
    {
        var result = _useCaseCollection.Request(request);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet("{paymentTransactionId:long}")]
    public IActionResult GetById(long paymentTransactionId)
    {
        return GetByIdResult(paymentTransactionId);
    }

    [HttpGet("order/{orderId:long}")]
    public IActionResult GetByOrder(long orderId)
    {
        var result = _useCaseCollection.GetByOrder(orderId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpPost("order/{orderId:long}/status/check")]
    public IActionResult CheckStatus(long orderId)
    {
        var result = _useCaseCollection.CheckStatus(orderId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
