using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.Interface;

public interface IUseCaseGet<TEntity>
    where TEntity : BaseEntity
{
    ListMessages Messages { get; }
    TEntity Execute(long entityId, IRepositoryQuery repositoryQuery);
}
