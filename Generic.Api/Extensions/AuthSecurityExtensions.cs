using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Generic.Api.Extensions;

public static class AuthSecurityExtensions
{
    public static IServiceCollection AddAuthSecurity<TCurrentUserContext>( this IServiceCollection services, IConfiguration configuration)
    {
        // Fazer com JwtBearer
        var secretKey = configuration.GetSection("AppSettings")["SecretKey"];  // Não esquecer de configurar isso no appsettings.json ou nas variáveis de ambiente
        var key = Encoding.ASCII.GetBytes(secretKey!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ClientOrAdmin", policy =>
                policy.RequireRole("Client", "Admin"));

            options.AddPolicy("SellerOrAdmin", policy =>
                policy.RequireRole("Seller", "Admin"));

            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
        });
        return services;
    }

}