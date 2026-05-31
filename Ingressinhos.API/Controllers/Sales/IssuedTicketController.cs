using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[ApiExplorerSettings(GroupName = "sales")]
[Route("api/[controller]")]
public class IssuedTicketsController : ApiQuery<IssuedTicket>
{
    private readonly IUseCaseIssuedTicketCollection _useCaseCollection;

    public IssuedTicketsController(IUseCaseIssuedTicketCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData(ODataQueryOptions<IssuedTicket> query)
    {
        return OData(query);
    }

    [HttpGet("me")]
    [Authorize(Policy = "OnlyClient")]
    public IActionResult GetMyTickets(
        [FromQuery(Name = "$skip")] int skip = 0,
        [FromQuery(Name = "$top")] int top = 50)
    {
        return ExecuteCustomData(_useCaseCollection.GetMyTickets(skip, top));
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }
}
