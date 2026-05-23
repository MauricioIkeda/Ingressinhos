using Generic.Api.Controllers;
using Ingressinhos.API.Extensions;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ApiCrud<Event, EventDto>
{
    private static readonly IEdmModel EventWithTicketsEdmModel = ODataExtensions.GetEventWithTicketsQueryEdmModel();
    private readonly IUseCaseEventCollection _eventCollection;

    public EventsController(IUseCaseEventCollection useCaseCollection) : base(useCaseCollection)
    {
        _eventCollection = useCaseCollection;
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetOData(ODataQueryOptions<Event> query)
    {
        return OData(query);
    }

    //[Authorize]
    [HttpGet("WithTickets")]
    public IActionResult GetWithTickets()
    {
        return OData(
            CreateQueryOptions<EventWithTicketsDto>(EventWithTicketsEdmModel),
            query => _eventCollection.GetWithTicketsResult(query));
    }

    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    [Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Include([FromBody] EventDto command)
    {
        return IncludeResultWithData(command);
    }

    [HttpPut]
    [Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Update([FromBody] EventDto command)
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
