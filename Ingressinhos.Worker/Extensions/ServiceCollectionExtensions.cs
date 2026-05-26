using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;
using Generic.Messaging.Services;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.UseCases;
using Ingressinhos.Application.Sales.UseCases;
using Ingressinhos.Infrastructure.Context;
using Ingressinhos.Infrastructure.Options;
using Ingressinhos.Infrastructure.ReadModels.TicketReadModel;
using Ingressinhos.Infrastructure.Services;
using Ingressinhos.Worker.Consumers.Payment;
using Ingressinhos.Worker.Consumers.TicketReadModel;
using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIngressinhosWorkerServices(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        services.AddIngressinhosDatabase(configuration);
        services.AddScoped<IUseCaseConfirmOrderPayment, ConfirmOrderPayment>();
        services.AddScoped<IUseCaseCancelOrderPayment, CancelOrderPayment>();
        services.AddScoped<IUseCaseIssueTicketsFromOrder, IssueTicketsFromOrder>();
        services.AddIngressinhosTicketReadModel(configuration);
        services.AddScoped<PaymentApprovedConsumer>();
        services.AddScoped<PaymentCancelledConsumer>();
        services.AddScoped<TicketReadModelSyncConsumer>();
        services.AddIngressinhosWorkerMessaging(configuration, contentRootPath);
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

    private static IServiceCollection AddIngressinhosWorkerMessaging(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        var provider = configuration["Messaging:Provider"];
        if (string.Equals(provider, "File", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton(CreateFileMessageBusOptions(configuration, contentRootPath));
            services.AddSingleton<IMessageConsumer, FileMessageConsumer>();
            services.AddSingleton<IMessagePublisher, FileMessagePublisher>();
            return services;
        }

        services.AddSingleton(CreateRabbitMqOptions(configuration));
        services.AddSingleton<IMessageConsumer, RabbitMqMessageConsumer>();
        services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();
        return services;
    }

    private static IServiceCollection AddIngressinhosTicketReadModel(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(CreateClientTicketMongoOptions(configuration));
        services.AddSingleton<MongoClientTicketReadModelRepository>();
        services.AddSingleton<IClientTicketReadModelWriter>(provider => provider.GetRequiredService<MongoClientTicketReadModelRepository>());
        services.AddSingleton<IClientTicketReadModelQuery>(provider => provider.GetRequiredService<MongoClientTicketReadModelRepository>());
        services.AddSingleton<IClientTicketReadModelHealthCheck>(provider => provider.GetRequiredService<MongoClientTicketReadModelRepository>());
        services.AddScoped<IClientTicketReadModelSyncPublisher, ClientTicketReadModelSyncPublisher>();

        services.AddScoped<ClientTicketReadModelBuilder>();
        services.AddScoped<IUseCaseProjectClientTicketsFromOrder, ProjectClientTicketsFromOrder>();
        services.AddScoped<IUseCaseProjectClientTicketFromIssuedTicket, ProjectClientTicketFromIssuedTicket>();
        services.AddScoped<IUseCaseRefreshClientTicketsByEvent, RefreshClientTicketsByEvent>();
        services.AddScoped<IUseCaseRefreshClientTicketsByLocation, RefreshClientTicketsByLocation>();
        services.AddScoped<IUseCaseBackfillClientTicketsReadModel, BackfillClientTicketsReadModel>();

        return services;
    }

    private static ClientTicketMongoOptions CreateClientTicketMongoOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection("Mongo");
        return new ClientTicketMongoOptions
        {
            ConnectionString = section["ConnectionString"] ?? "mongodb://localhost:27017",
            Database = section["Database"] ?? "IngressinhosReadDb",
            TicketCollection = section["TicketCollection"] ?? "clientTickets"
        };
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
