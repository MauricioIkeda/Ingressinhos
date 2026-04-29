using Generic.Domain.Entities;
using System.Linq.Expressions;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    ListMessages Messages { get; }
    IQueryable<TEntity> GetOdata(Expression<Func<TEntity, bool>> where);
    TEntity GetById(long id);
}
