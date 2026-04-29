using Generic.Domain.Entities;
using System.Linq.Expressions;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    OperationResult<IQueryable<TEntity>> GetOdata(Expression<Func<TEntity, bool>> where);
    OperationResult<TEntity> GetById(long id);
}
