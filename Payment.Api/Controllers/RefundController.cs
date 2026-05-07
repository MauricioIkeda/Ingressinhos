using Generic.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/payments/refunds")]
public class RefundController : ApiQuery<Refund>
{
    private readonly IUseCaseRefundCollection _useCaseCollection;

    public RefundController(IUseCaseRefundCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return QueryAllResult();
    }

    [HttpPost]
    public IActionResult RequestRefund([FromBody] RequestRefundDto request)
    {
        var result = _useCaseCollection.Request(request);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpGet("{refundId:long}")]
    public IActionResult GetById(long refundId)
    {
        return GetByIdResult(refundId);
    }

    [HttpGet("payment/{paymentTransactionId:long}")]
    public IActionResult GetByPaymentTransaction(long paymentTransactionId)
    {
        var result = _useCaseCollection.GetByPaymentTransaction(paymentTransactionId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpPost("{refundId:long}/status/check")]
    public IActionResult CheckStatus(long refundId)
    {
        var result = _useCaseCollection.CheckStatus(refundId);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
