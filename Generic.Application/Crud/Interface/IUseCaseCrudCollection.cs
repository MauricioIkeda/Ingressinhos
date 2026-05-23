using Generic.Domain.Entities;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseCrudCollection<TEntity, TCommand> : IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    OperationResult Include(TCommand command);
    OperationResult<TCommand> IncludeWithResult(TCommand command);
    OperationResult Update(TCommand command);
    OperationResult Delete(long id);
}
