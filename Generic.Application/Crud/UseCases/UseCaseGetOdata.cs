using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace Generic.Application.Crud.UseCases;

public class UseCaseGetOdata<TEntity>
    where TEntity : BaseEntity
{

    public virtual IQueryable<TEntity> Execute(Expression<Func<TEntity, bool>> where, IRepositoryQuery repositoryQuery)
    {

            return repositoryQuery.Query<TEntity>().Where(where);
    }
}
