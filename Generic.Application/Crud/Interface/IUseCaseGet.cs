using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseGet<TEntity>
    where TEntity : BaseEntity
{
    OperationResult<TEntity> Execute(long entityId, IRepositoryQuery repositoryQuery);
}
