using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/[controller]")]
public class OrderItemsController : ApiQuery<OrderItem>
{
    public OrderItemsController(IUseCaseOrderItemCollection useCaseCollection) : base(useCaseCollection)
    {
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData(ODataQueryOptions<OrderItem> query)
    {
        return OData(query);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }
}
