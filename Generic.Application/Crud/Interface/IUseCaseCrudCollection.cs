using Generic.Domain.Entities;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseCrudCollection<TEntity, in TCommand> : IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    OperationResult Include(TCommand command);
    OperationResult Update(TCommand command);
    OperationResult Delete(long id);
}
