using Generic.Api.Controllers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[Route("api/seats")]
public class SeatController : ApiCrud<Seat, SeatDto>
{
    public SeatController(IUseCaseSeatCollection useCaseCollection)
        : base(useCaseCollection)
    {
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return QueryAllResult();
    }

    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    public IActionResult Include([FromBody] SeatDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    public IActionResult Update([FromBody] SeatDto command)
    {
        return UpdateResult(command);
    }

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return DeleteResult(id);
    }
}
