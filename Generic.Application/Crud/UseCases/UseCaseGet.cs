using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseGet<TEntity> : IUseCaseGet<TEntity>
    where TEntity : BaseEntity
{
    public OperationResult<TEntity> Execute(long entityId, IRepositoryQuery repositoryQuery)
    {
        if (entityId <= 0)
        {
            return OperationResult<TEntity>.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador."));
        }

        var entity = repositoryQuery.Return<TEntity>(entityId);

        if (entity is null)
        {
            return OperationResult<TEntity>.NotFound(new MensagemErro("Id", "Nada foi encontrado."));
        }

        return OperationResult<TEntity>.Ok(entity);
    }
}
