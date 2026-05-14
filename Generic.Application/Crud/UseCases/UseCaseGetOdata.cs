using System.Linq.Expressions;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseGetOdata<TEntity> 
    where TEntity : BaseEntity
{
    public virtual List<TEntity> Execute(Expression<Func<TEntity, bool>> where, IRepositoryQuery repositoryQuery)
    {
        return repositoryQuery.Query<TEntity>().Where(where).ToList();
    }

    public virtual List<TOutput> Execute<TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> transaction, IRepositoryQuery repositoryQuery)
    {
        return transaction(repositoryQuery.Query<TEntity>()).ToList();
    }
}
