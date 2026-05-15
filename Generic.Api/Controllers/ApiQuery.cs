using System.Collections;
using Generic.Api.Dtos;
using Generic.Application.Crud.Interface;
using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Wrapper;

namespace Generic.Api.Controllers;

public abstract partial class ApiQuery<TEntity> : ControllerBase
    where TEntity : BaseEntity
{
    private readonly IUseCaseQueryCollection<TEntity> _queryCollection;
    protected readonly ODataQuerySettings DefaultOdataSettings;
    // No fluxo com $expand, algumas opcoes sao consumidas no primeiro passo e nao devem ser reaplicadas depois.
    protected AllowedQueryOptions CustomIgnore { get; set; } = AllowedQueryOptions.Filter | AllowedQueryOptions.Top | AllowedQueryOptions.Skip;

    protected ApiQuery(IUseCaseQueryCollection<TEntity> queryCollection)
    {
        _queryCollection = queryCollection;
        DefaultOdataSettings = new ODataQuerySettings
        {
            HandleNullPropagation = HandleNullPropagationOption.False
        };
    }

    protected IActionResult OData(ODataQueryOptions<TEntity> queryOptions)
    {
        long? count = null;
        var result = _queryCollection.GetDynamic(query => DefaultGetOData(queryOptions, query, out count));
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(StatusCodes.Status200OK, GetResultOData(result.Data, count));
    }

    protected IActionResult GetByIdResult(long id)
    {
        return ExecuteCustomData(_queryCollection.GetById(id));
    }

    protected IActionResult ExecuteCustom(OperationResult result)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode);
    }

    protected IActionResult ExecuteCustomData<T>(OperationResult<T> result)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(result.StatusCode, result.Data);
    }

    protected virtual ResultOData<object> GetResultOData(List<object> result, long? count)
    {
        // OData pode devolver wrappers internos quando ha $select/$expand; aqui normalizamos para objetos simples.
        var results = result?.Select(UnwrapSelectExpandWrapper).ToList() ?? [];
        return new ResultOData<object>(count ?? results.Count, results);
    }

    protected virtual IQueryable<object> QueryWithSelectExpand(ODataQueryOptions<TEntity> queryOptions, IQueryable<TEntity> query)
    {
        if (queryOptions is null)
        {
            return query.Cast<object>();
        }

        var hasExpand = !string.IsNullOrEmpty(queryOptions.SelectExpand?.RawExpand);

        if (hasExpand)
        {
            // Com $expand fazemos em duas etapas:
            // 1. aplicamos a consulta para descobrir quais entidades entraram no resultado;
            // 2. buscamos novamente a partir dos ids para montar o payload final com os relacionamentos.
            var ids = ApplyTo<TEntity>(query, AllowedQueryOptions.Expand | AllowedQueryOptions.Select)
                .Select(entity => entity.Id)
                .ToArray();

            return ApplyTo<object>(query.Where(entity => ids.Contains(entity.Id)), CustomIgnore);
        }

        return ApplyTo<object>(query);

        IQueryable<TOutput> ApplyTo<TOutput>(IQueryable<TEntity> source, AllowedQueryOptions ignoredOptions = AllowedQueryOptions.None)
        {
            var queryable = queryOptions.ApplyTo(source, DefaultOdataSettings, ignoredOptions);
            return (queryable as IQueryable<TOutput>) ?? queryable.Cast<TOutput>();
        }
    }

    protected virtual IQueryable<object> DefaultGetOData(ODataQueryOptions<TEntity> queryOptions, IQueryable<TEntity> query, out long? count)
    {
        // Prepara a consulta OData e calcula o total quando $count for solicitado.
        count = null;

        if (queryOptions is null)
        {
            // Sem OData, reaproveita a query base.
            return query.Cast<object>();
        }

        if (queryOptions.Count is not null)
        {
            // O count considera o filtro atual, sem materializar os dados aqui.
            IQueryable filteredQuery = queryOptions.Filter?.ApplyTo(query, DefaultOdataSettings) ?? query;
            count = queryOptions.Count.GetEntityCount(filteredQuery);
        }

        // Monta a query final; a execucao acontece depois, no ToList da consulta dinamica.
        return QueryWithSelectExpand(queryOptions, query);
    }

    private object UnwrapSelectExpandWrapper(object item)
    {
        // OData usa ISelectExpandWrapper para representar projecoes dinamicas.
        // Convertendo recursivamente para dicionarios/listas simples, a serializacao fica previsivel.
        if (item is ISelectExpandWrapper selectExpandWrapper)
        {
            IDictionary<string, object> dictionary = selectExpandWrapper.ToDictionary();

            foreach (var key in dictionary.Keys.ToList())
            {
                var value = dictionary[key];

                if (value is ISelectExpandWrapper nestedWrapper)
                {
                    dictionary[key] = UnwrapSelectExpandWrapper(nestedWrapper);
                    continue;
                }

                if (value is List<object> list)
                {
                    dictionary[key] = list.Select(UnwrapSelectExpandWrapper).ToList();
                    continue;
                }

                if (value is IEnumerable enumerable && value is not string)
                {
                    var values = new List<object>();
                    foreach (var itemValue in enumerable)
                    {
                        values.Add(UnwrapSelectExpandWrapper(itemValue));
                    }

                    if (values.Count > 0)
                    {
                        dictionary[key] = values;
                    }
                }
            }

            return dictionary;
        }

        return item;
    }
}

