using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Generic.Api.Controllers;

public abstract class ApiQuery<TEntity> : ControllerBase
    where TEntity : BaseEntity
{
    private readonly IUseCaseQueryCollection<TEntity> _queryCollection;

    protected ApiQuery(IUseCaseQueryCollection<TEntity> queryCollection)
    {
        _queryCollection = queryCollection;
    }

    protected IActionResult QueryAllResult()
    {
        return QueryResult(_ => true);
    }

    protected IActionResult QueryResult(Expression<Func<TEntity, bool>> where)
    {
        var result = _queryCollection.GetOdata(where);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data.ToList());
    }

    protected IActionResult GetByIdResult(long id)
    {
        var result = _queryCollection.GetById(id);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
