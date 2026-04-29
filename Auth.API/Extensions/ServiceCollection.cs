using Auth.Application.Authorization.UserAccess.Interfaces;
using Auth.Application.Authorization.UserAccess.UseCases;
using Auth.Application.Utils.Interface;
using Auth.Application.Utils.Services;
using Auth.Infrastructure.Context;
using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthDatabase(configuration);
        services.AddAuthApplicationUseCases();
        return services;
    }

    public static IServiceCollection AddAuthDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var authConnectionString = configuration.GetConnectionString("AuthConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'AuthConnection' nao foi configurada.");

        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(authConnectionString));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<AuthDbContext>());
        services.AddScoped<IRepositorySession, RepositorySessionEF>();

        return services;
    }

    public static IServiceCollection AddAuthApplicationUseCases(this IServiceCollection services)
    {
        services.AddScoped<IUseCaseUserAccessQuery, UseCaseUserAccessQuery>();
        services.AddScoped<IUseCaseUserAuthCollection, AuthenticateUserUseCase>();
        services.AddScoped<IToken>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            return new Token(
                config["AppSettings:SecretKey"],
                config["AppSettings:Issuer"],
                config["AppSettings:Audience"]
            );
        });

        return services;
    }
}
