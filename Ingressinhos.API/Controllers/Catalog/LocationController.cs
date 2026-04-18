using Generic.Api.Controllers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Location.Dtos;
using Ingressinhos.Application.Catalog.Location.UseCases;
using Ingressinhos.Domain.Catalog.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[Route("[controller]")]
public class LocationController : ControllerBase
{
    private CreateLocationUseCase _createLocationUseCase;
    
    public LocationController(CreateLocationUseCase createLocationUseCase)
    {
        _createLocationUseCase = createLocationUseCase;
    }

    [HttpPost("/CreateLocation")]
    public IActionResult Create([FromBody] LocationDto locationDto)
    {
        var entidade = _createLocationUseCase.Execute(locationDto);
            
        return Ok(entidade);
    }
}