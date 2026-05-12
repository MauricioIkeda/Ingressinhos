using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;

namespace Generic.Api.Extensions;

public static class SlugifyRouteExtensions
{
    public static void AddSlugifiedRoutes(this MvcOptions options)
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new Transformer()));
    }

    private sealed class Transformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value is null)
            {
                return null;
            }

            return Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        }
    }
}
