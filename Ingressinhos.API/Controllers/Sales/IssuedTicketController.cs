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
public class IssuedTicketsController : ApiCrud<IssuedTicket, IssuedTicketDto>
{
    public IssuedTicketsController(IUseCaseIssuedTicketCollection useCaseCollection) : base(useCaseCollection)
    {
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData(ODataQueryOptions<IssuedTicket> query)
    {
        return OData(query);
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Include([FromBody] IssuedTicketDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Update([FromBody] IssuedTicketDto command)
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
