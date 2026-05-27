using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[ApiExplorerSettings(GroupName = "sales")]
[Route("api/[controller]")]
public class OrdersController : ApiQuery<Order>
{
    private readonly IUseCaseOrderCollection _useCaseCollection;

    public OrdersController(IUseCaseOrderCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData(ODataQueryOptions<Order> query)
    {
        return OData(query);
    }

    [HttpGet("cart/{clientId:long?}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult GetCurrentCart(long? clientId = null)
    {
        return ExecuteCustomData(_useCaseCollection.GetCurrentCart(clientId ?? 0));
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost("cart/items")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult AddCartItem([FromBody] AddCartItemRequest command)
    {
        return ExecuteCustom(_useCaseCollection.AddCartItem(command));
    }

    [HttpPatch("{id:long}/close")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Close(long id)
    {
        return ExecuteCustomData(_useCaseCollection.Close(id));
    }

    [HttpPost("immediate")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Immediate([FromBody] CreateOrderRequest command)
    {
        return ExecuteCustomData(_useCaseCollection.Immediate(command));
    }

    [HttpDelete("cart/reset/{clientId:long?}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult ResetCart(long? clientId = null)
    {
        return ExecuteCustom(_useCaseCollection.ResetCart(clientId ?? 0));
    }

    [HttpDelete("cart/items/{orderItemId:long}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult RemoveCartItem(long orderItemId)
    {
        return ExecuteCustom(_useCaseCollection.RemoveCartItem(orderItemId));
    }
}
