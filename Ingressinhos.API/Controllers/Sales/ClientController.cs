using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/clients")]
public class ClientController : ApiCrud<Client, ClientDto>
{
    private readonly IUseCaseClientCollection _useCaseCollection;

    public ClientController(IUseCaseClientCollection useCaseCollection) : base(useCaseCollection)
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

    private IActionResult ExecuteCustom(OperationResult result)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode);
    }
}
