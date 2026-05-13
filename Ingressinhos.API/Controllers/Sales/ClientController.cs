using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ApiCrud<Client, ClientDto>
{
    private readonly IUseCaseClientCollection _useCaseCollection;

    public ClientsController(IUseCaseClientCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
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
    [Authorize(Policy = "ClientOrAdmin")]
    public IActionResult Deactivate(long id)
    {
        return ExecuteCustom(_useCaseCollection.Deactivate(id));
    }

    [HttpDelete("{id:long}/hard")]  // excluir esse endpoint quando finalizar
    public IActionResult HardDelete(long id)
    {
        return ExecuteCustom(_useCaseCollection.Delete(id));
    }

    [HttpPatch("{id:long}/recover")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Recover(long id)
    {
        return ExecuteCustom(_useCaseCollection.Recover(id));
    }

    [Authorize(Policy = "ClientOrAdmin")]
    [HttpGet("me")]
    public IActionResult GetByToken()
    {
        var result = _useCaseCollection.GetByToken();
        if (result.Success)
            return Ok(result.Data);
        else
            return BadRequest(result.Errors);
    }
}
