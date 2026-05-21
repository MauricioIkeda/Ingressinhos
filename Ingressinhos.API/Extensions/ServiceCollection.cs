using Generic.Application.Utils.Interface;
using Generic.Application.Utils.Services;
using Generic.Application.Crud.Interface;
using Generic.Application.Crud.UseCases;
using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Application.Catalog.Location.UseCases;
using Ingressinhos.Application.Catalog.UseCases;
using Ingressinhos.Application.Sales.Interfaces;
using Ingressinhos.Application.Sales.UseCases;
using Ingressinhos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIngressinhosServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
        services.AddIngressinhosDatabase(configuration);
        services.AddIngressinhosApplicationUseCases();
        services.AddIngressinhosAuthClient(configuration);
        services.AddIngressinhosPaymentClient(configuration);
        return services;
    }

    public static IServiceCollection AddIngressinhosDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nao foi configurada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IRepositorySession, RepositorySessionEF>();
        services.AddScoped<IRepositoryQuery, RepositoryQueryEF>();
        services.AddScoped<IRepository, RepositoryEF>();

        return services;
    }

    public static IServiceCollection AddIngressinhosApplicationUseCases(this IServiceCollection services)
    {
        services.AddScoped<EventInclude>();
        services.AddScoped<EventUpdate>();
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

        services.AddScoped<CreateOrder>();
        services.AddScoped<UpdateOrderItems>();
        services.AddScoped<IUseCaseCloseOrder, CloseOrder>();
        services.AddScoped<IUseCaseImmediateOrder, ImmediateOrder>();
        services.AddScoped<IUseCaseDelete<Ingressinhos.Domain.Sales.Entities.Order>, UseCaseDelete<Ingressinhos.Domain.Sales.Entities.Order>>();
        services.AddScoped<IUseCaseConfirmOrderPayment, ConfirmOrderPayment>();
        services.AddScoped<IUseCaseCancelOrderPayment, CancelOrderPayment>();
        services.AddScoped<IUseCaseOrderCollection, UseCaseOrderCollection>();

        services.AddScoped<OrderItemGetOdata>();
        services.AddScoped<IUseCaseOrderItemCollection, UseCaseOrderItemCollection>();

        services.AddScoped<IssuedTicketInclude>();
        services.AddScoped<IssuedTicketUpdate>();
        services.AddScoped<IUseCaseIssueTicketsFromOrder, IssueTicketsFromOrder>();
        services.AddScoped<IUseCaseIssuedTicketCollection, UseCaseIssuedTicketCollection>();

        return services;
    }

    public static IServiceCollection AddIngressinhosAuthClient(this IServiceCollection services, IConfiguration configuration)
    {
        var authApiBaseUrl = configuration["AuthApi:BaseUrl"];
        if (string.IsNullOrWhiteSpace(authApiBaseUrl))
        {
            throw new InvalidOperationException("AuthApi:BaseUrl nao foi configurado. Ex.: http://localhost:5254");
        }

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
