using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
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
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPaymentDatabase(configuration);
        services.AddPaymentUseCases();
        services.AddScoped<IPaymentProcessor, RandomMockPaymentProcessor>();
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
        services.AddScoped<IUseCasePaymentTransactionCollection, UseCasePaymentTransactionCollection>();
        services.AddScoped<IUseCaseRequestRefund, RequestRefund>();
        services.AddScoped<IUseCaseGetRefund, GetRefund>();
        services.AddScoped<IUseCaseGetRefundsByPaymentTransaction, GetRefundsByPaymentTransaction>();
        services.AddScoped<IUseCaseCheckRefundStatus, CheckRefundStatus>();
        services.AddScoped<IUseCaseRefundCollection, UseCaseRefundCollection>();
        return services;
    }
}
