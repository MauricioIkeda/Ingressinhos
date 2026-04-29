using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Generic.Api.Controllers;

public abstract class ApiCrud<TEntity, TCommand> : ApiQuery<TEntity>
    where TEntity : BaseEntity
{
    private readonly IUseCaseCrudCollection<TEntity, TCommand> _crudCollection;

    protected ApiCrud(IUseCaseCrudCollection<TEntity, TCommand> crudCollection)
        : base(crudCollection)
    {
        _crudCollection = crudCollection;
    }

    protected IActionResult IncludeResult(TCommand command)
    {
        return ExecuteCommand(
            () => _crudCollection.Include(command),
            $" Incluido com sucesso.",
            $" Não foi possivel incluir.");
    }

    protected IActionResult UpdateResult(TCommand command)
    {
        return ExecuteCommand(
            () => _crudCollection.Update(command),
            $" Atualizado com sucesso.",
            $" Não foi possivel atualizar.");
    }

    protected IActionResult DeleteResult(long id)
    {
        return ExecuteCommand(
            () => _crudCollection.Delete(id),
            $" Removido com sucesso.",
           $" Não foi possivel remover.");
    }

    private IActionResult ExecuteCommand(Func<bool> commandAction, string successMessage, string errorMessage)
    {
        if (commandAction is null)
        {
            return BadRequestFromMessages("A acao de comando nao foi configurada.");
        }

        ResetMessages();

        var result = commandAction();

        if (!result)
        {
            return BadRequestFromMessages(errorMessage);
        }

        return OkFromMessages(successMessage);
    }
}
