using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using System.Linq.Expressions;

namespace Generic.Application.Crud.UseCases;

public class UseCaseQueryCollection<TEntity> : IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    private readonly UseCaseGetOdata<TEntity> _useCaseGetOdata;
    private readonly IUseCaseGet<TEntity> _useCaseGetById;
    protected readonly IRepositorySession _repositorySession;

    public UseCaseQueryCollection(UseCaseGetOdata<TEntity> useCaseGetOdata, IUseCaseGet<TEntity> useCaseGetById, IRepositorySession repositorySession)
    {
        _useCaseGetOdata = useCaseGetOdata;
        _useCaseGetById = useCaseGetById;
        _repositorySession = repositorySession;
    }

    public virtual OperationResult<IQueryable<TEntity>> GetOdata(Expression<Func<TEntity, bool>> where)
    {
        var result = _useCaseGetOdata.Execute(where, _repositorySession.GetRepositoryQuery());
        return OperationResult<IQueryable<TEntity>>.Ok(result);
    }

    public virtual OperationResult<TEntity> GetById(long id)
    {
        return _useCaseGetById.Execute(id, _repositorySession.GetRepositoryQuery());
    }
}
