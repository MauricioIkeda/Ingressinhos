using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public abstract class UseCaseGetQueryItems<TEntity, TQueryItem>
    where TEntity : BaseEntity
{
    private readonly IRepositoryQuery _repositoryQuery;

    protected UseCaseGetQueryItems(IRepositoryQuery repositoryQuery)
    {
        _repositoryQuery = repositoryQuery;
    }

    protected abstract TQueryItem ToQueryItem(TEntity entity);

    public virtual OperationResult<List<TOutput>> Execute<TOutput>(Func<IQueryable<TQueryItem>, IQueryable<TOutput>> transaction)
    {
        if (transaction is null)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(new MensagemErro("Filtro", "Transformacao da consulta deve ser informada."));
        }

        try
        {
            var query = _repositoryQuery
                .Query<TEntity>()
                .ToList()
                .Select(ToQueryItem)
                .AsQueryable();

            var result = transaction(query).ToList();
            return OperationResult<List<TOutput>>.Ok(result);
        }
        catch (Exception ex)
        {
            return OperationResult<List<TOutput>>.UnprocessableEntity(MensagemErro.Geral(ex.Message));
        }
    }
}
