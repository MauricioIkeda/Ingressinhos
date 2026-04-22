using Generic.Application.Interface;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Generic.Api.Controllers;

public abstract class ApiQuery<TEntity> : ControllerBase
    where TEntity : BaseEntity
{
    private readonly IUseCaseQueryCollection<TEntity> _queryCollection;

    protected ApiQuery(IUseCaseQueryCollection<TEntity> queryCollection)
    {
        _queryCollection = queryCollection;
    }

    protected IActionResult QueryAllResult()
    {
        return QueryResult(_ => true);
    }

    protected IActionResult QueryResult(Expression<Func<TEntity, bool>> where)
    {
        if (where is null)
        {
            return BadRequest(CreateMessages($"Filtro de consulta de deve ser informado."));
        }

        try
        {
            ResetMessages();
            var result = _queryCollection.GetOdata(where).ToList();
            return Ok(result);
        }
        catch
        {
            return BadRequest(MessagesOrFallback($"Nao foi possivel consultar."));
        }
    }

    protected IActionResult GetByIdResult(long id)
    {
        try
        {
            ResetMessages();
            var result = _queryCollection.GetById(id);
            return Ok(result);
        }
        catch
        {
            return BadRequest(MessagesOrFallback($"Nao foi possivel buscar {typeof(TEntity).Name}."));
        }
    }

    protected IActionResult OkFromMessages(string successMessage)
    {
        if (!_queryCollection.Messages.Any())
        {
            _queryCollection.Messages.Add(successMessage);
        }

        return Ok(_queryCollection.Messages);
    }

    protected IActionResult BadRequestFromMessages(string fallbackMessage)
    {
        return BadRequest(MessagesOrFallback(fallbackMessage));
    }

    protected void ResetMessages()
    {
        _queryCollection.Messages.Clear();
    }

    private ListMessages MessagesOrFallback(string fallbackMessage)
    {
        if (_queryCollection.Messages.Any())
        {
            return _queryCollection.Messages;
        }

        return CreateMessages(fallbackMessage, error: true);
    }

    private static ListMessages CreateMessages(string message, bool error = true)
    {
        var messages = new ListMessages();
        messages.Add(message, error);
        return messages;
    }
}
