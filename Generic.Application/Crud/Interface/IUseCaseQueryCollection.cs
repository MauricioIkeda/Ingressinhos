using System.Linq.Expressions;
using Generic.Domain.Entities;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    OperationResult<List<TEntity>> Get(Expression<Func<TEntity, bool>> where);
    OperationResult<List<TOutput>> GetDynamic<TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> transaction);
    OperationResult<TEntity> GetById(long id);
}
