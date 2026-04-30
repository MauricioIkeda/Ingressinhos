using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/orders")]
public class OrderController : ApiCrud<Order, OrderDto>
{
    public OrderController(IUseCaseOrderCollection useCaseCollection) : base(useCaseCollection)
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
    public IActionResult Include([FromBody] OrderDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Update([FromBody] OrderDto command)
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
