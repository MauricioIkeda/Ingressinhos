using System.Security.Claims;
using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace Generic.Api.Middlewares;

//Mauricio deixei uns comentarios ai para voce, agradeça
public static class RateLimitExtensions
{
    private static readonly ConcurrentDictionary<string, DateTimeOffset> WindowStarts = new();
    private const int DefaultGetPermitLimit = 45; // Padrão caso nao venha nada para GET
    private const int DefaultNonGetPermitLimit = 15; // Padrão caso não venha nada para metodos diferentes de GET
    private const int DefaultWindowInSeconds = 60; // Padrão caso nao venha nada

    public static IServiceCollection AddHttpMethodRateLimiting( // Limite de requisições baseado no metodo HTTP e User 
        this IServiceCollection services,
        IConfiguration? configuration = null)
    {
        var getRateLimit = GetRateLimitSettings( // Quantia de requisições GET permitidas e janela de tempo
            configuration,
            "RateLimiting:Get",
            DefaultGetPermitLimit);

        var nonGetRateLimit = GetRateLimitSettings( // Quantia de requisições nao-GET permitidas e janela de tempo
            configuration,
            "RateLimiting:NonGet",
            DefaultNonGetPermitLimit);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) => // retorno quando limite atingido
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                bool isGet = HttpMethods.IsGet(context.HttpContext.Request.Method);
                RateLimitSettings rateLimit = isGet ? getRateLimit : nonGetRateLimit;
                string partitionKey = GetPartitionKey(context.HttpContext, isGet ? "get" : "non-get"); // Separa o contador de GET e nao-GET

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    message = $"Muitas requisições. Tente novamente em breve.",
                    statusCode = StatusCodes.Status429TooManyRequests
                }, cancellationToken);
            };
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context => // Partição baseada no token e no verbo
            {
                bool isGet = HttpMethods.IsGet(context.Request.Method);
                RateLimitSettings rateLimit = isGet ? getRateLimit : nonGetRateLimit;
                string partitionKey = GetPartitionKey(context, isGet ? "get" : "non-get"); // Separa o contador de GET e nao-GET

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = rateLimit.PermitLimit,
                        QueueLimit = 0,
                        Window = rateLimit.Window
                    });
            });
        });

        return services;
    }

    private static string GetPartitionKey(HttpContext context, string methodGroup) // Pegar o identificador ou IP
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); // Limite colocado em cima do identificador do token
        if (!string.IsNullOrWhiteSpace(userId))
        {
            return $"{methodGroup}:user:{userId}";
        }

        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString(); // Caso não tenha um token, pega o IP
        return !string.IsNullOrWhiteSpace(remoteIpAddress)
            ? $"{methodGroup}:ip:{remoteIpAddress}"
            : $"{methodGroup}:ip:unknown";
    }

    private static RateLimitSettings GetRateLimitSettings(
        IConfiguration? configuration,
        string sectionPath,
        int defaultPermitLimit)
    {
        var permitLimit = GetPositiveInteger( // Quantia de requisições permitidas
            configuration,
            $"{sectionPath}:PermitLimit",
            defaultPermitLimit);

        var windowInSeconds = GetPositiveInteger( // Janela de tempo que vamos usar
            configuration,
            $"{sectionPath}:WindowInSeconds",
            DefaultWindowInSeconds);

        return new RateLimitSettings(permitLimit, TimeSpan.FromSeconds(windowInSeconds));
    }

    private static int GetPositiveInteger(IConfiguration? configuration, string key, int defaultValue) // Pegar o appsetting, ou padrao
    {
        if (configuration is null)
        {
            return defaultValue;
        }

        return int.TryParse(configuration[key], out var value) && value > 0
            ? value
            : defaultValue;
    }

    private sealed record RateLimitSettings(int PermitLimit, TimeSpan Window); // Dto das configurações de janela de tempo e quantidade de acessos
}
