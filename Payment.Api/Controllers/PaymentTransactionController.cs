using Generic.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Payment.Api.Extensions;
using Payment.Aplication.Transactions.Dtos;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Domain.Entities;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/payments/transactions")]
public class PaymentTransactionController : ApiQuery<PaymentTransaction>
{
    private static readonly IEdmModel PaymentTransactionQueryEdmModel = ODataExtensions.GetPaymentTransactionQueryEdmModel();
    private readonly IUseCasePaymentTransactionCollection _useCaseCollection;
    private readonly IUseCaseSimulatePaymentWebhook _simulatePaymentWebhook;

    public PaymentTransactionController(
        IUseCasePaymentTransactionCollection useCaseCollection,
        IUseCaseSimulatePaymentWebhook simulatePaymentWebhook) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
        _simulatePaymentWebhook = simulatePaymentWebhook;
    }

    [HttpGet]
    public IActionResult GetOData()
    {
        return OData(
            CreateQueryOptions<PaymentTransactionQueryItem>(
                PaymentTransactionQueryEdmModel,
                ("Amount/Value", "Amount")),
            query => _useCaseCollection.GetQueryItems(query));
    }

    [HttpPost]
    public IActionResult RequestPayment([FromBody] RequestPaymentDto request)
    {
        return ExecuteCustomData(_useCaseCollection.Request(request));
    }

    [HttpGet("{paymentTransactionId:long}")]
    public IActionResult GetById(long paymentTransactionId)
    {
        return GetByIdResult(paymentTransactionId);
    }

    [HttpGet("order/{orderId:long}")]
    public IActionResult GetByOrder(long orderId)
    {
        return ExecuteCustomData(_useCaseCollection.GetByOrder(orderId));
    }

    [HttpPost("order/{orderId:long}/status/check")]
    public IActionResult CheckStatus(long orderId)
    {
        return ExecuteCustomData(_useCaseCollection.CheckStatus(orderId));
    }

    [HttpPost("{paymentTransactionId:long}/simulate-webhook")]
    public IActionResult SimulateWebhook(long paymentTransactionId, [FromBody] SimulatePaymentWebhookDto request)
    {
        var result = _simulatePaymentWebhook.Execute(paymentTransactionId, request);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode);
    }
}
