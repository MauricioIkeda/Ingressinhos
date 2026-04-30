using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/clients")]
public class ClientController : ApiCrud<Client, ClientDto>
{
    public ClientController(IUseCaseClientCollection useCaseCollection) : base(useCaseCollection)
    {
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAll()
    {
        return QueryAllResult();
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult GetById(long id)
    {
        return GetByIdResult(id);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Include([FromBody] ClientDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Update([FromBody] ClientDto command)
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
