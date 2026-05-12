using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ApiCrud<OrderItem, OrderItemDto>
{
    public OrderItemsController(IUseCaseOrderItemCollection useCaseCollection) : base(useCaseCollection)
    {
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAll()
    {
        return QueryAllResult();
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Include([FromBody] OrderItemDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Update([FromBody] OrderItemDto command)
    {
        return UpdateResult(command);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Delete(long id)
    {
        return DeleteResult(id);
    }
}
