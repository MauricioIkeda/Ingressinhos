using Generic.Api.Controllers;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[Route("api/[controller]")]
public class SellersController : ApiCrud<Seller, SellerDto>
{
    private readonly IUseCaseSellerCollection _useCaseCollection;

    public SellersController(IUseCaseSellerCollection useCaseCollection) : base(useCaseCollection)
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
    [Authorize(Policy = "SellerOrAdmin")]
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
}
