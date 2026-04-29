using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public class UseCaseGet<TEntity> : IUseCaseGet<TEntity>
    where TEntity : BaseEntity
{
    public ListMessages Messages { get; } = new();

    public TEntity Execute(long entityId, IRepositoryQuery repositoryQuery)
    {
        Messages.Clear();

        if (entityId <= 0)
        {
            Messages.Add("Deve ser informado o identificador", error: true);
            return null;
        }

        var entity = repositoryQuery.Return<TEntity>(entityId);

        if (entity is null)
        {
            Messages.Add("Nada foi encontrado", error: true);
            return null;
        }

        return entity;
    }
}
