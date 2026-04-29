using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseDelete<TEntity> : IUseCaseDelete<TEntity>
    where TEntity : BaseEntity
{
    public virtual OperationResult Execute(long entityId, IRepositorySession repositorySession)
    {
        if (entityId <= 0)
        {
            return OperationResult.UnprocessableEntity(new MensagemErro("Id", "Deve ser informado o identificador."));
        }

        var entity = repositorySession.GetRepositoryQuery().Return<TEntity>(entityId);
        if (entity == null)
        {
            return OperationResult.NotFound(new MensagemErro("Id", "Nada encontrado."));
        }

        var repository = repositorySession.GetRepository();
        repository.Delete(entity);
        repository.Flush().GetAwaiter().GetResult();
        return OperationResult.Ok();
    }
}
