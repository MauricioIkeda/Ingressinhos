using System.Linq.Expressions;
using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseQueryCollection<TEntity> : IUseCaseQueryCollection<TEntity>
    where TEntity : BaseEntity
{
    private readonly UseCaseGetOdata<TEntity> _useCaseGetOdata;
    private readonly IUseCaseGetId<TEntity> _useCaseGetById;
    protected readonly IRepositorySession _repositorySession;

    public UseCaseQueryCollection(UseCaseGetOdata<TEntity> useCaseGetOdata, IUseCaseGetId<TEntity> useCaseGetById, IRepositorySession repositorySession)
    {
        _useCaseGetOdata = useCaseGetOdata;
        _useCaseGetById = useCaseGetById;
        _repositorySession = repositorySession;
    }

    public virtual OperationResult<List<TEntity>> Get(Expression<Func<TEntity, bool>> where) // Esse seria o getodata
    {
        if (where is null)
        {
            return OperationResult<List<TEntity>>.UnprocessableEntity(new MensagemErro("Filtro", "Filtro de consulta deve ser informado."));
        }

        try
        {
            var result = _useCaseGetOdata.Execute(where, _repositorySession.GetRepositoryQuery());
            return OperationResult<List<TEntity>>.Ok(result);
        }
        catch (Exception ex)
        {
            return OperationResult<List<TEntity>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    public virtual OperationResult<List<TOutput>> GetDynamic<TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> transaction) // Esse seria o getodata com select e expand, no caso poderia ter join, join left e todo o resto da sacanagem
    {
        if (transaction is null)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(new MensagemErro("Filtro", "Transformacao da consulta deve ser informada."));
        }

        try
        {
            var result = _useCaseGetOdata.Execute(transaction, _repositorySession.GetRepositoryQuery());
            return OperationResult<List<TOutput>>.Ok(result);
        }
        catch (Exception ex)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }

    public virtual OperationResult<TEntity> GetById(long id)
    {
        return _useCaseGetById.Execute(id, _repositorySession.GetRepositoryQuery());
    }
}
