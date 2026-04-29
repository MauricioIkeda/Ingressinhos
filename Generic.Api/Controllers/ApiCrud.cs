using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Generic.Api.Controllers;

public abstract class ApiCrud<TEntity, TCommand> : ApiQuery<TEntity>
    where TEntity : BaseEntity
{
    private readonly IUseCaseCrudCollection<TEntity, TCommand> _crudCollection;

    protected ApiCrud(IUseCaseCrudCollection<TEntity, TCommand> crudCollection)
        : base(crudCollection)
    {
        _crudCollection = crudCollection;
    }

    protected IActionResult IncludeResult(TCommand command)
    {
        return ExecuteCommand(_crudCollection.Include(command));
    }

    protected IActionResult UpdateResult(TCommand command)
    {
        return ExecuteCommand(_crudCollection.Update(command));
    }

    protected IActionResult DeleteResult(long id)
    {
        return ExecuteCommand(_crudCollection.Delete(id));
    }

    private IActionResult ExecuteCommand(OperationResult result)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode);
    }
}
