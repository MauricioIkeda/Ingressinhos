using Generic.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Generic.Api.Controllers;

public abstract partial class ApiQuery<TEntity>
    where TEntity : BaseEntity
{
    // Versao usada quando a consulta roda sobre um read model plano, em vez da entidade do dominio.
    protected IActionResult OData<TQueryItem>(
        ODataQueryOptions<TQueryItem> queryOptions,
        Func<Func<IQueryable<TQueryItem>, IQueryable<object>>, OperationResult<List<object>>> queryExecutor)
    {
        long? count = null;
        var result = queryExecutor(query => DefaultGetOData(queryOptions, query, out count));
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, result.Errors);
        }

        return StatusCode(StatusCodes.Status200OK, GetResultOData(result.Data, count));
    }

    protected ODataQueryOptions<TQueryItem> CreateQueryOptions<TQueryItem>(
        IEdmModel edmModel,
        params (string From, string To)[] queryStringReplacements)
    {
        // Monta um ODataQueryOptions manualmente sobre o EDM do read model.
        // Isso permite expor um shape plano para consulta sem acoplar o controller ao parsing da query.
        var context = new ODataQueryContext(edmModel, typeof(TQueryItem), new ODataPath());
        var httpContext = new DefaultHttpContext
        {
            RequestServices = HttpContext.RequestServices
        };

        httpContext.Request.Method = Request.Method;
        httpContext.Request.Scheme = Request.Scheme;
        httpContext.Request.Host = Request.Host;
        httpContext.Request.PathBase = Request.PathBase;
        httpContext.Request.Path = Request.Path;
        httpContext.Request.QueryString = NormalizeQueryString(Request.QueryString, queryStringReplacements);

        return new ODataQueryOptions<TQueryItem>(context, httpContext.Request);
    }

    protected static QueryString NormalizeQueryString(
        QueryString queryString,
        params (string From, string To)[] queryStringReplacements)
    {
        // Mantem compatibilidade com queries antigas, como Cpf/Numero, enquanto o endpoint novo expoe apenas Cpf.
        if (!queryString.HasValue || queryStringReplacements.Length == 0)
        {
            return queryString;
        }

        var normalizedValue = queryString.Value!;
        foreach (var (from, to) in queryStringReplacements)
        {
            normalizedValue = normalizedValue.Replace(from, to, StringComparison.OrdinalIgnoreCase);
        }

        return new QueryString(normalizedValue);
    }

    protected virtual IQueryable<object> DefaultGetOData<TQueryItem>(
        ODataQueryOptions<TQueryItem> queryOptions,
        IQueryable<TQueryItem> query,
        out long? count)
    {
        // Para read models planos o ApplyTo direto costuma ser suficiente, sem o tratamento especial de $expand.
        count = null;

        if (queryOptions is null)
        {
            return query.Cast<object>();
        }

        if (queryOptions.Count is not null)
        {
            IQueryable filteredQuery = queryOptions.Filter?.ApplyTo(query, DefaultOdataSettings) ?? query;
            count = queryOptions.Count.GetEntityCount(filteredQuery);
        }

        var queryable = queryOptions.ApplyTo(query, DefaultOdataSettings);
        return (queryable as IQueryable<object>) ?? queryable.Cast<object>();
    }
}

