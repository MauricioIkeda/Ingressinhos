using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.UseCases;

public class UseCaseGet<TEntity>
    where TEntity : BaseEntity
{

    public TEntity Execute(long entityId, IRepositoryQuery repositoryQuery)
    {
        if (entityId <= 0)
        {
            throw new Exception("Deve ser informado o identificador");
        }

        var entity = repositoryQuery.Return<TEntity>(entityId);

        if (entity is null)
        {
            throw new Exception("Nada foi encontrado");
        }

        return entity;
    }
}
