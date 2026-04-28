using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseDelete<TEntity>
    where TEntity : BaseEntity
{
    public virtual bool Execute(long entityId, IRepositorySession repositorySession)
    {
        if (entityId <= 0)
        {
            throw new Exception("Deve ser informado o identificador");
        }
        TEntity entity = repositorySession.GetRepositoryQuery().Return<TEntity>(entityId);
        if (entity == null)
        {
            throw new Exception("Nada encontrado");
        }

        IRepository repository = repositorySession.GetRepository();
        repository.Delete(entity);
        repository.Flush().GetAwaiter().GetResult();
        return true;
    }
}
