using Generic.Api.Controllers;
using Ingressinhos.API.Extensions;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[ApiExplorerSettings(GroupName = "sales")]
[Route("api/[controller]")]
public class OrderItemsController : ApiQuery<OrderItem>
{
    private static readonly IEdmModel OrderItemQueryEdmModel = ODataExtensions.GetOrderItemQueryEdmModel();
    private readonly IUseCaseOrderItemCollection _useCaseCollection;

    public OrderItemsController(IUseCaseOrderItemCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData()
    {
        return OData(
            CreateQueryOptions<OrderItemQueryItem>(
                OrderItemQueryEdmModel,
                ("UnitPrice/Value", "UnitPrice")),
            query => _useCaseCollection.GetQueryItems(query));
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }
}
