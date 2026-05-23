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
        return ExecuteCustom(_crudCollection.Include(command));
    }

    protected IActionResult IncludeResultWithData(TCommand command)
    {
        return ExecuteCustomData(_crudCollection.IncludeWithResult(command));
    }

    protected IActionResult UpdateResult(TCommand command)
    {
        return ExecuteCustom(_crudCollection.Update(command));
    }

    protected IActionResult DeleteResult(long id)
    {
        return ExecuteCustom(_crudCollection.Delete(id));
    }
}
