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

    public virtual bool Include(TCommand command)
    {
        Messages.Clear();

        var result = _include.Execute(command);

        if (_include.Messages.Any())
        {
            Messages.AddRange(_include.Messages);
        }

        if (!result)
        {
            return false;
        }

        if (!Messages.Any())
        {
            Messages.Add($"{typeof(TEntity).Name} incluido com sucesso.");
        }

        return true;
    }

    public virtual bool Update(TCommand command)
    {
        Messages.Clear();

        var result = _update.Execute(command);

        if (_update.Messages.Any())
        {
            Messages.AddRange(_update.Messages);
        }

        if (!result)
        {
            return false;
        }

        if (!Messages.Any())
        {
            Messages.Add($"{typeof(TEntity).Name} atualizado com sucesso.");
        }

        return true;
    }

    public virtual bool Delete(long id)
    {
        Messages.Clear();

        var result = _useCaseDelete.Execute(id, _repositorySession);

        if (_useCaseDelete.Messages.Any())
        {
            Messages.AddRange(_useCaseDelete.Messages);
        }

        if (!result)
        {
            return false;
        }

        if (!Messages.Any())
        {
            Messages.Add($"{typeof(TEntity).Name} removido com sucesso.");
        }

        return true;
    }
}
