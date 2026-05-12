using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;
using Generic.Messaging.Services;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Application.Sales.UseCases;
using Ingressinhos.Infrastructure.Context;
using Ingressinhos.Worker.Consumers;
using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIngressinhosWorkerServices(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        services.AddIngressinhosDatabase(configuration);
        services.AddScoped<IUseCaseConfirmOrderPayment, ConfirmOrderPayment>();
        services.AddScoped<IUseCaseIssueTicketsFromOrder, IssueTicketsFromOrder>();
        services.AddScoped<PaymentApprovedConsumer>();
        services.AddSingleton(CreateFileMessageBusOptions(configuration, contentRootPath));
        services.AddSingleton<IMessageConsumer, FileMessageConsumer>();
        return services;
    }

    private static IServiceCollection AddIngressinhosDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao foi configurada.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IRepositorySession, RepositorySessionEF>();

        return services;
    }

    private static FileMessageBusOptions CreateFileMessageBusOptions(IConfiguration configuration, string contentRootPath)
    {
        var configuredBasePath = configuration["Messaging:BasePath"];
        var basePath = string.IsNullOrWhiteSpace(configuredBasePath)
            ? Path.GetFullPath(Path.Combine(contentRootPath, "..", "message-bus"))
            : Path.GetFullPath(Path.IsPathRooted(configuredBasePath)
                ? configuredBasePath
                : Path.Combine(contentRootPath, configuredBasePath));

        return new FileMessageBusOptions
        {
            BasePath = basePath
        };
    }
}
