using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;

namespace Generic.Application.Crud.UseCases;

public abstract class UseCaseCrudCollection<TEntity, TCommand> : UseCaseQueryCollection<TEntity>, IUseCaseCrudCollection<TEntity, TCommand>
    where TEntity : BaseEntity
{
    private readonly UseCaseDelete<TEntity> _useCaseDelete;
    private readonly Func<TCommand, bool> _include;
    private readonly Func<TCommand, bool> _update;

    protected UseCaseCrudCollection( Func<TCommand, bool> include, Func<TCommand, bool> update, UseCaseGetOdata<TEntity> useCaseGetOdata, 
        UseCaseGet<TEntity> useCaseGetById, UseCaseDelete<TEntity> useCaseDelete, IRepositorySession repositorySession)
        : base(useCaseGetOdata, useCaseGetById, repositorySession)
    {
        _include = include;
        _update = update;
        _useCaseDelete = useCaseDelete;
    }

    public virtual bool Include(TCommand command)
    {
        try
        {
            var result = _include(command);

            if (!result)
            {
                Messages.Add($"{typeof(TEntity).Name} nao foi incluido.", error: true);
                return false;
            }

            Messages.Add($"{typeof(TEntity).Name} incluido com sucesso.");
            return result;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }

    public virtual bool Update(TCommand command)
    {
        try
        {
            var result = _update(command);

            if (!result)
            {
                Messages.Add($"{typeof(TEntity).Name} nao foi atualizado.", error: true);
                return false;
            }

            Messages.Add($"{typeof(TEntity).Name} atualizado com sucesso.");
            return result;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }

    public virtual bool Delete(long id)
    {
        try
        {
            var result = _useCaseDelete.Execute(id, _repositorySession);

            if (!result)
            {
                Messages.Add($"{typeof(TEntity).Name} nao foi removido.", error: true);
                return false;
            }

            Messages.Add($"{typeof(TEntity).Name} removido com sucesso.");
            return true;
        }
        catch (Exception ex)
        {
            Messages.Add(ex);
            return false;
        }
    }
}