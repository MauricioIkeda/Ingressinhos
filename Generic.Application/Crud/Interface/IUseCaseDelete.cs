using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseDelete<TEntity>
    where TEntity : BaseEntity
{
    OperationResult Execute(long entityId, IRepositorySession repositorySession);
}
