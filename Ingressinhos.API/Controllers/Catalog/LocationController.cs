using Generic.Api.Controllers;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Application.Catalog.Location.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[ApiExplorerSettings(GroupName = "catalog")]
[Route("api/[controller]")]
public class LocationsController : ApiCrud<Location, LocationDto>
{
    public LocationsController(IUseCaseLocationCollection useCaseCollection) : base(useCaseCollection)
    {
    }

    [HttpGet]
    //[Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData(ODataQueryOptions<Location> query)
    {
        return OData(query);
    }

    [HttpGet("{id:long}")]
    //[Authorize(Policy = "AdminOnly")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    //[Authorize(Policy = "AdminOnly")]
    public IActionResult Include([FromBody] LocationDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Update([FromBody] LocationDto command)
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
