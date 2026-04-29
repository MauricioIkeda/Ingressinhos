using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseDelete<TEntity> : IUseCaseDelete<TEntity>
    where TEntity : BaseEntity
{
    public ListMessages Messages { get; } = new();

    public virtual bool Execute(long entityId, IRepositorySession repositorySession)
    {
        Messages.Clear();

        if (entityId <= 0)
        {
            Messages.Add("Deve ser informado o identificador", error: true);
            return false;
        }

        var entity = repositorySession.GetRepositoryQuery().Return<TEntity>(entityId);
        if (entity == null)
        {
            Messages.Add("Nada encontrado", error: true);
            return false;
        }

        var repository = repositorySession.GetRepository();
        repository.Delete(entity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
