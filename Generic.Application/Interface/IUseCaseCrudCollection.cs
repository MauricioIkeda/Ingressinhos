using Generic.Domain.Entities;

namespace Generic.Application.Interface;

public interface IUseCaseCrudCollection<TEntity, in TCommand> : IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    bool Include(TCommand command);
    bool Update(TCommand command);
    bool Delete(long id);
}