using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;
using Generic.Messaging.Services;
using Microsoft.EntityFrameworkCore;
using Payment.Aplication.Refunds.Interfaces;
using Payment.Aplication.Refunds.UseCases;
using Payment.Aplication.Transactions.Interfaces;
using Payment.Aplication.Transactions.UseCases;
using Payment.Infrastructure.Context;
using Payment.Infrastructure.Services;

namespace Payment.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        services.AddHttpContextAccessor();
        services.AddPaymentDatabase(configuration);
        services.AddPaymentUseCases();
        services.AddPaymentMessaging(configuration, contentRootPath);
        services.AddScoped<IPaymentProcessor, RandomMockPaymentProcessor>();
        services.AddScoped<IMockCheckoutUrlBuilder, MockCheckoutUrlBuilder>();
        return services;
    }

    public static IServiceCollection AddPaymentDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'PaymentConnection' nao foi configurada.");

        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<PaymentDbContext>());
        services.AddScoped<IRepository>(sp => new RepositoryEF(sp.GetRequiredService<DbContext>()));
        services.AddScoped<IRepositoryQuery>(sp => new RepositoryQueryEF(sp.GetRequiredService<DbContext>()));
        services.AddScoped<IRepositorySession, RepositorySessionEF>();

        return services;
    }

    public static IServiceCollection AddPaymentUseCases(this IServiceCollection services)
    {
        services.AddScoped<IUseCaseRequestPayment, RequestPayment>();
        services.AddScoped<IUseCaseGetPaymentTransaction, GetPaymentTransaction>();
        services.AddScoped<IUseCaseGetPaymentsByOrder, GetPaymentsByOrder>();
        services.AddScoped<IUseCaseCheckPaymentStatus, CheckPaymentStatus>();
        services.AddScoped<IUseCaseHandlePaymentNotification, HandlePaymentNotification>();
        services.AddScoped<IUseCaseSimulatePaymentWebhook, SimulatePaymentWebhook>();
        services.AddScoped<PaymentTransactionGetOdata>();
        services.AddScoped<IUseCasePaymentTransactionCollection, UseCasePaymentTransactionCollection>();
        services.AddScoped<IUseCaseRequestRefund, RequestRefund>();
        services.AddScoped<IUseCaseGetRefund, GetRefund>();
        services.AddScoped<IUseCaseGetRefundsByPaymentTransaction, GetRefundsByPaymentTransaction>();
        services.AddScoped<IUseCaseCheckRefundStatus, CheckRefundStatus>();
        services.AddScoped<RefundGetOdata>();
        services.AddScoped<IUseCaseRefundCollection, UseCaseRefundCollection>();
        return services;
    }

    private static IServiceCollection AddPaymentMessaging(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        var provider = configuration["Messaging:Provider"];
        if (string.Equals(provider, "File", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton(CreateFileMessageBusOptions(configuration, contentRootPath));
            services.AddSingleton<IMessagePublisher, FileMessagePublisher>();
            return services;
        }

        services.AddSingleton(CreateRabbitMqOptions(configuration));
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
        return services;
    }

    private static RabbitMqOptions CreateRabbitMqOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection("Messaging:RabbitMq");
        return new RabbitMqOptions
        {
            HostName = section["HostName"] ?? "localhost",
            Port = int.TryParse(section["Port"], out var port) ? port : 5672,
            UserName = section["UserName"] ?? "guest",
            Password = section["Password"] ?? "guest",
            VirtualHost = section["VirtualHost"] ?? "/",
            RequeueOnFailure = bool.TryParse(section["RequeueOnFailure"], out var requeueOnFailure) && requeueOnFailure,
            PrefetchCount = ushort.TryParse(section["PrefetchCount"], out var prefetchCount) ? prefetchCount : (ushort)10,
            MaxMessagesPerBatch = int.TryParse(section["MaxMessagesPerBatch"], out var maxMessagesPerBatch)
                ? maxMessagesPerBatch
                : 50
        };
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
