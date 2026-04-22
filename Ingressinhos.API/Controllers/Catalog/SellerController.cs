using Generic.Api.Controllers;
using Generic.Application.Interface;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Domain.Catalog.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Ingressinhos.API.Controllers.Catalog;

[ApiController]
[Route("api/sellers")]
public class SellerController : ApiCrud<Seller, SellerDto>
{
    public SellerController(IUseCaseSellerCollection useCaseCollection)
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
    public IActionResult Include([FromBody] SellerDto command)
    {
        return IncludeResult(command);
    }

    [HttpPut]
    public IActionResult Update([FromBody] SellerDto command)
    {
        return UpdateResult(command);
    }

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return DeleteResult(id);
    }
}
