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

    public ListMessages Messages { get; } = new();

    public UseCaseQueryCollection(UseCaseGetOdata<TEntity> useCaseGetOdata, IUseCaseGet<TEntity> useCaseGetById, IRepositorySession repositorySession)
    {
        _useCaseGetOdata = useCaseGetOdata;
        _useCaseGetById = useCaseGetById;
        _repositorySession = repositorySession;
    }

    public virtual IQueryable<TEntity> GetOdata(Expression<Func<TEntity, bool>> where)
    {
        Messages.Clear();
        return _useCaseGetOdata.Execute(where, _repositorySession.GetRepositoryQuery());
    }

    public virtual TEntity GetById(long id)
    {
        Messages.Clear();

        var result = _useCaseGetById.Execute(id, _repositorySession.GetRepositoryQuery());

        if (_useCaseGetById.Messages.Any())
        {
            Messages.AddRange(_useCaseGetById.Messages);
        }

        return result;
    }
}
