using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public abstract class UseCaseCrudCollection<TEntity, TCommand> : UseCaseQueryCollection<TEntity>, IUseCaseCrudCollection<TEntity, TCommand>
    where TEntity : BaseEntity
{
    private readonly IUseCaseDelete<TEntity> _useCaseDelete;
    private readonly IUseCaseCommand<TCommand> _include;
    private readonly IUseCaseCommand<TCommand> _update;

    protected UseCaseCrudCollection(
        IUseCaseCommand<TCommand> include,
        IUseCaseCommand<TCommand> update,
        UseCaseGetOdata<TEntity> useCaseGetOdata,
        IUseCaseGet<TEntity> useCaseGetById,
        IUseCaseDelete<TEntity> useCaseDelete,
        IRepositorySession repositorySession)
        : base(useCaseGetOdata, useCaseGetById, repositorySession)
    {
        _include = include;
        _update = update;
        _useCaseDelete = useCaseDelete;
    }

    public virtual OperationResult Include(TCommand command)
    {
        return _include.Execute(command);
    }

    public virtual OperationResult Update(TCommand command)
    {
        return _update.Execute(command);
    }

    public virtual OperationResult Delete(long id)
    {
        return _useCaseDelete.Execute(id, _repositorySession);
    }
}
