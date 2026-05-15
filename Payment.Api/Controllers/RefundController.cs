using Generic.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Payment.Api.Extensions;
using Payment.Aplication.Refunds.Dtos;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/payments/refunds")]
public class RefundController : ApiQuery<Refund>
{
    private static readonly IEdmModel RefundQueryEdmModel = ODataExtensions.GetRefundQueryEdmModel();
    private readonly IUseCaseRefundCollection _useCaseCollection;

    public RefundController(IUseCaseRefundCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    public IActionResult GetOData()
    {
        return OData(
            CreateQueryOptions<RefundQueryItem>(
                RefundQueryEdmModel,
                ("Amount/Value", "Amount")),
            query => _useCaseCollection.GetQueryItems(query));
    }

    [HttpPost]
    public IActionResult RequestRefund([FromBody] RequestRefundDto request)
    {
        return ExecuteCustomData(_useCaseCollection.Request(request));
    }

    [HttpGet("{refundId:long}")]
    public IActionResult GetById(long refundId)
    {
        return GetByIdResult(refundId);
    }

    [HttpGet("payment/{paymentTransactionId:long}")]
    public IActionResult GetByPaymentTransaction(long paymentTransactionId)
    {
        return ExecuteCustomData(_useCaseCollection.GetByPaymentTransaction(paymentTransactionId));
    }

    [HttpPost("{refundId:long}/status/check")]
    public IActionResult CheckStatus(long refundId)
    {
        return ExecuteCustomData(_useCaseCollection.CheckStatus(refundId));
    }
}
