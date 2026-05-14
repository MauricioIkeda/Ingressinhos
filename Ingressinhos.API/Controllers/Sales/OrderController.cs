using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
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

    [HttpGet("{id:long}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Include([FromBody] CreateOrderRequest command)
    {
        return ExecuteCustom(_useCaseCollection.Create(command));
    }

    [HttpPatch("{id:long}/close")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Close(long id)
    {
        return ExecuteCustomData(_useCaseCollection.Close(id));
    }

    [HttpPut]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Update([FromBody] UpdateOrderItemsRequest command)
    {
        return ExecuteCustom(_useCaseCollection.UpdateItems(command));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Delete(long id)
    {
        return ExecuteCustom(_useCaseCollection.Delete(id));
    }
}
