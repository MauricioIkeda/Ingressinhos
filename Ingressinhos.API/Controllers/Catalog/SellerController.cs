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
[ApiExplorerSettings(GroupName = "catalog")]
[Route("api/[controller]")]
public class SellersController : ApiCrud<Seller, SellerDto>
{
    private static readonly IEdmModel SellerQueryEdmModel = ODataExtensions.GetSellerQueryEdmModel();
    private readonly IUseCaseSellerCollection _useCaseCollection;

    public SellersController(IUseCaseSellerCollection useCaseCollection) : base(useCaseCollection)
    {
        _useCaseCollection = useCaseCollection;
    }

    [HttpGet]
    //[Authorize(Policy = "AdminOnly")]
    public IActionResult GetOData()
    {
        return OData( CreateQueryOptions<SellerQueryItem>(
                SellerQueryEdmModel,
                ("Cnpj/Numero", "Cnpj"), // estamos fazendo isso para porque
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
    public IActionResult Include([FromBody] SellerDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    [Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Update([FromBody] SellerDto command)
    {
        return UpdateResult(command);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "SellerOrAdmin")]
    public IActionResult Deactivate(long id)
    {
        return ExecuteCustom(_useCaseCollection.Deactivate(id));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPatch("{id:long}/recover")]
    public IActionResult Recover(long id)
    {
        return ExecuteCustom(_useCaseCollection.Recover(id));
    }

    [Authorize(Policy = "OnlySeller")] // Somente para o corno do vendedor
    [HttpGet("me")]
    public IActionResult GetByToken()
    {
        return ExecuteCustomData(_useCaseCollection.GetByToken());
    }
}
