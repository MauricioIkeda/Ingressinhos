using Generic.Api.Controllers;
using Ingressinhos.API.Extensions;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[ApiExplorerSettings(GroupName = "sales")]
[Route("api/[controller]")]
public class IssuedTicketsController : ApiQuery<IssuedTicket>
{
    private static readonly IEdmModel ClientTicketViewEdmModel = ODataExtensions.GetClientTicketViewEdmModel();
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
        DefaultOdataSettings.PageSize = 100;

        return OData(
            CreateQueryOptions<ClientTicketViewDto>(ClientTicketViewEdmModel),
            query => _useCaseCollection.GetMyTickets(query));
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }
}
