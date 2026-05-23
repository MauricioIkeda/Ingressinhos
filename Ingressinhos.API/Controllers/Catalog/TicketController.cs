using Generic.Api.Controllers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.API.Extensions;
using Ingressinhos.Domain.Catalog.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ApiCrud<Ticket, TicketDto>
{
    private static readonly IEdmModel TicketQueryEdmModel = ODataExtensions.GetTicketQueryEdmModel();
    private readonly IUseCaseTicketCollection _useCaseCollection;

    public TicketsController(IUseCaseTicketCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    public IActionResult GetOData()
    {
        return OData(
            CreateQueryOptions<TicketQueryItem>(
                TicketQueryEdmModel,
                ("BasePrice/Value", "BasePrice"),
                ("PremiumPrice/Value", "PremiumPrice"),
                ("VIPPrice/Value", "VIPPrice")),
            query => _useCaseCollection.GetQueryItems(query));
    }

    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    //[Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Include([FromBody] TicketDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Update([FromBody] TicketDto command)
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
