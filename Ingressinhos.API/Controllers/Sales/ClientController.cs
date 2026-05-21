using Generic.Api.Controllers;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Ingressinhos.API.Extensions;

namespace Ingressinhos.API.Controllers.Sales;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ApiCrud<Client, ClientDto>
{
    private static readonly IEdmModel ClientQueryEdmModel = ODataExtensions.GetClientQueryEdmModel();
    private readonly IUseCaseClientCollection _useCaseCollection;

    public ClientsController(IUseCaseClientCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData()
    {
        return OData(
            CreateQueryOptions<ClientQueryItem>(
                ClientQueryEdmModel,
                ("Cpf/Numero", "Cpf"),
                ("Email/Endereco", "Email")),
            query => _useCaseCollection.GetQueryItems(query));
    }

    [HttpGet("{id:long}")]
    [Authorize]
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

    [Authorize(Policy = "OnlyClient")] // somente para o corno do clientex e do front-end
    [HttpGet("me")]
    public IActionResult GetByToken()
    {
        return ExecuteCustom(_useCaseCollection.GetByToken());
    }
}
