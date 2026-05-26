using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
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
    public IActionResult GetMyTickets()
    {
        return ExecuteCustomData(_useCaseCollection.GetMyTickets());
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }
}
