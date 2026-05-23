using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public abstract class UseCaseCrudCollection<TEntity, TCommand> : UseCaseQueryCollection<TEntity>, IUseCaseCrudCollection<TEntity, TCommand>
    where TEntity : BaseEntity
{
    private readonly IUseCaseDelete<TEntity> _useCaseDelete;
    private readonly IUseCaseCommand<TCommand> _include;
    private readonly IUseCaseCommand<TCommand, TCommand> _includeWithResult;
    private readonly IUseCaseCommand<TCommand> _update;

    protected UseCaseCrudCollection(
        IUseCaseCommand<TCommand> include,
        IUseCaseCommand<TCommand> update,
        UseCaseGetOdata<TEntity> useCaseGetOdata,
        IUseCaseGetId<TEntity> useCaseGetById,
        IUseCaseDelete<TEntity> useCaseDelete,
        IRepositorySession repositorySession)
        : base(useCaseGetOdata, useCaseGetById, repositorySession)
    {
        _include = include;
        _includeWithResult = include as IUseCaseCommand<TCommand, TCommand>;
        _update = update;
        _useCaseDelete = useCaseDelete;
    }

    protected UseCaseCrudCollection(
        IUseCaseCommand<TCommand> include,
        IUseCaseCommand<TCommand> update,
        UseCaseGetOdata<TEntity> useCaseGetOdata,
        IUseCaseGetId<TEntity> useCaseGetById,
        IUseCaseDelete<TEntity> useCaseDelete,
        IRepositorySession repositorySession,
        IReadRepositoryQuery readRepositoryQuery)
        : base(useCaseGetOdata, useCaseGetById, repositorySession, readRepositoryQuery)
    {
        _include = include;
        _includeWithResult = include as IUseCaseCommand<TCommand, TCommand>;
        _update = update;
        _useCaseDelete = useCaseDelete;
    }

    public virtual OperationResult Include(TCommand command)
    {
        return _include.Execute(command);
    }

    public virtual OperationResult<TCommand> IncludeWithResult(TCommand command)
    {
        return _includeWithResult?.Execute(command) ?? OperationResult<TCommand>.FromResult(_include.Execute(command));
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
