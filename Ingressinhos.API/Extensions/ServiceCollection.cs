using Generic.Application.Utils.Interface;
using Generic.Application.Utils.Services;
using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Generic.Messaging.Interfaces;
using Generic.Messaging.Options;
using Generic.Messaging.Services;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Application.Catalog.Location.UseCases;
using Ingressinhos.Application.Catalog.UseCases;
using Ingressinhos.Application.Onboarding.UseCases;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.Interfaces;
using Ingressinhos.Application.Sales.TicketReadModel.UseCases;
using Ingressinhos.Application.Sales.UseCases;
using Ingressinhos.Infrastructure.Context;
using Ingressinhos.Infrastructure.Options;
using Ingressinhos.Infrastructure.ReadModels.TicketReadModel;
using Ingressinhos.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIngressinhosServices(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
        services.AddIngressinhosDatabase(configuration);
        services.AddIngressinhosMessaging(configuration, contentRootPath);
        services.AddIngressinhosTicketReadModel(configuration);
        services.AddIngressinhosApplicationUseCases();
        services.AddIngressinhosSentinelAuthClient(configuration);
        services.AddIngressinhosPaymentClient(configuration);
        return services;
    }

    public static IServiceCollection AddIngressinhosDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao foi configurada.");
        // O fallback preserva ambientes antigos, mas o desenvolvimento com replica deve
        // configurar explicitamente a ReadConnection para nao mascarar falhas de replicacao.
        var readConnectionString = configuration.GetConnectionString("ReadConnection") ?? connectionString;

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<ReadAppDbContext>(options =>
            options
                .UseNpgsql(readConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IRepositorySession, RepositorySessionEF>();
        services.AddScoped<IRepositoryQuery, RepositoryQueryEF>();
        services.AddScoped<IReadRepositoryQuery>(provider =>
            new ReadRepositoryQueryEF(provider.GetRequiredService<ReadAppDbContext>()));
        services.AddScoped<IRepository, RepositoryEF>();

        return services;
    }

    public static IServiceCollection AddIngressinhosApplicationUseCases(this IServiceCollection services)
    {
        services.AddScoped<EventInclude>();
        services.AddScoped<EventUpdate>();
        services.AddScoped<EventGetWithTickets>();
        services.AddScoped<EventGetSeats>();
        services.AddScoped<IUseCaseEventCollection, UseCaseEventCollection>();

        services.AddScoped<CreateLocationUseCase>();
        services.AddScoped<UpdateLocationUseCase>();
        services.AddScoped<IUseCaseLocationCollection, UseCaseLocationCollection>();

        services.AddScoped<SeatInclude>();
        services.AddScoped<SeatUpdate>();
        services.AddScoped<IUseCaseSeatCollection, UseCaseSeatCollection>();

        services.AddScoped<TicketInclude>();
        services.AddScoped<TicketUpdate>();
        services.AddScoped<TicketGetOdata>();
        services.AddScoped<IUseCaseTicketCollection, UseCaseTicketCollection>();

        services.AddScoped<SellerInclude>();
        services.AddScoped<SellerUpdate>();
        services.AddScoped<SellerDeactivate>();
        services.AddScoped<SellerRecover>();
        services.AddScoped<SellerGetByToken>();
        services.AddScoped<SellerGetOdata>();
        services.AddScoped<IUseCaseSellerCollection, UseCaseSellerCollection>();

        services.AddScoped<ClientInclude>();
        services.AddScoped<ClientUpdate>();
        services.AddScoped<ClientDeactivate>();
        services.AddScoped<ClientRecover>();
        services.AddScoped<ClientGetByToken>();
        services.AddScoped<ClientGetOdata>();
        services.AddScoped<IUseCaseClientCollection, UseCaseClientCollection>();

        services.AddScoped<AddCartItem>();
        services.AddScoped<RemoveCartItem>();
        services.AddScoped<ResetCart>();
        services.AddScoped<GetCurrentCart>();
        services.AddScoped<IUseCaseCloseOrder, CloseOrder>();
        services.AddScoped<IUseCaseImmediateOrder, ImmediateOrder>();
        services.AddScoped<IUseCaseConfirmOrderPayment, ConfirmOrderPayment>();
        services.AddScoped<IUseCaseCancelOrderPayment, CancelOrderPayment>();
        services.AddScoped<IUseCaseOrderCollection, UseCaseOrderCollection>();

        services.AddScoped<OrderItemGetOdata>();
        services.AddScoped<IUseCaseOrderItemCollection, UseCaseOrderItemCollection>();

        services.AddScoped<IssuedTicketInclude>();
        services.AddScoped<IssuedTicketUpdate>();
        services.AddScoped<IUseCaseIssueTicketsFromOrder, IssueTicketsFromOrder>();
        services.AddScoped<IUseCaseIssuedTicketCollection, UseCaseIssuedTicketCollection>();

        services.AddScoped<ClientTicketReadModelBuilder>();
        services.AddScoped<IUseCaseProjectClientTicketsFromOrder, ProjectClientTicketsFromOrder>();
        services.AddScoped<IUseCaseProjectClientTicketFromIssuedTicket, ProjectClientTicketFromIssuedTicket>();
        services.AddScoped<IUseCaseRefreshClientTicketsByEvent, RefreshClientTicketsByEvent>();
        services.AddScoped<IUseCaseRefreshClientTicketsByLocation, RefreshClientTicketsByLocation>();
        services.AddScoped<IUseCaseBackfillClientTicketsReadModel, BackfillClientTicketsReadModel>();
        services.AddScoped<IUseCaseGetMyClientTickets, GetMyClientTickets>();

        services.AddScoped<ProfileStatusUseCase>();
        services.AddScoped<OnboardClientUseCase>();
        services.AddScoped<OnboardSellerUseCase>();

        return services;
    }

    public static IServiceCollection AddIngressinhosTicketReadModel(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(CreateClientTicketMongoOptions(configuration));
        services.AddSingleton<MongoClientTicketReadModelRepository>();
        services.AddSingleton<IClientTicketReadModelWriter>(provider => provider.GetRequiredService<MongoClientTicketReadModelRepository>());
        services.AddSingleton<IClientTicketReadModelQuery>(provider => provider.GetRequiredService<MongoClientTicketReadModelRepository>());
        services.AddSingleton<IClientTicketReadModelHealthCheck>(provider => provider.GetRequiredService<MongoClientTicketReadModelRepository>());
        services.AddScoped<IClientTicketReadModelSyncPublisher, ClientTicketReadModelSyncPublisher>();

        return services;
    }

    public static IServiceCollection AddIngressinhosMessaging(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
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

    public static IServiceCollection AddIngressinhosSentinelAuthClient(this IServiceCollection services, IConfiguration configuration)
    {
        var authSection = configuration.GetSection("SentinelAuthClient");
        var authApiBaseUrl = authSection["BaseUrl"];
        if (string.IsNullOrWhiteSpace(authApiBaseUrl))
        {
            throw new InvalidOperationException("SentinelAuthClient:BaseUrl nao foi configurado. Ex.: http://localhost:5254");
        }

        services.AddSingleton(new SentinelAuthClientOptions
        {
            ApplicationClientId = long.TryParse(authSection["ApplicationClientId"], out var applicationClientId) ? applicationClientId : 0,
            ClientId = authSection["ClientId"] ?? string.Empty,
            AdminRoleId = long.TryParse(authSection["AdminRoleId"], out var adminRoleId) ? adminRoleId : 0,
            SellerRoleId = long.TryParse(authSection["SellerRoleId"], out var sellerRoleId) ? sellerRoleId : 0,
            ClientRoleId = long.TryParse(authSection["ClientRoleId"], out var clientRoleId) ? clientRoleId : 0
        });

        services.AddHttpClient<IRequestAuth, RequestAuth>(client =>
        {
            client.BaseAddress = new Uri(authApiBaseUrl);
        });

        return services;
    }

    public static IServiceCollection AddIngressinhosPaymentClient(this IServiceCollection services, IConfiguration configuration)
    {
        var paymentApiBaseUrl = configuration["PaymentApi:BaseUrl"];
        if (string.IsNullOrWhiteSpace(paymentApiBaseUrl))
        {
            throw new InvalidOperationException("PaymentApi:BaseUrl nao foi configurado. Ex.: http://localhost:5000");
        }

        services.AddHttpClient<IRequestPayment, RequestPayment>(client =>
        {
            client.BaseAddress = new Uri(paymentApiBaseUrl);
        });

        return services;
    }
}
