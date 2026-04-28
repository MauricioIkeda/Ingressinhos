using Generic.Api.Extensions;
using Generic.Application.Utils.Interface;
using Generic.Application.Utils.UseCase;
using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Ingressinhos.Application.Catalog.Interfaces;
using Ingressinhos.Application.Catalog.Location.UseCases;
using Ingressinhos.Application.Catalog.UseCases;
using Ingressinhos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Ingressinhos API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IRepositorySession, RepositorySessionEF>();
builder.Services.AddScoped<EventInclude>();
builder.Services.AddScoped<EventUpdate>();
builder.Services.AddScoped<IUseCaseEventCollection, UseCaseEventCollection>();

builder.Services.AddScoped<CreateLocationUseCase>();
builder.Services.AddScoped<UpdateLocationUseCase>();
builder.Services.AddScoped<IUseCaseLocationCollection, UseCaseLocationCollection>();

builder.Services.AddScoped<SeatInclude>();
builder.Services.AddScoped<SeatUpdate>();
builder.Services.AddScoped<IUseCaseSeatCollection, UseCaseSeatCollection>();

builder.Services.AddScoped<TicketInclude>();
builder.Services.AddScoped<TicketUpdate>();
builder.Services.AddScoped<IUseCaseTicketCollection, UseCaseTicketCollection>();

builder.Services.AddScoped<SellerInclude>();
builder.Services.AddScoped<SellerUpdate>();
builder.Services.AddScoped<IUseCaseSellerCollection, UseCaseSellerCollection>();
builder.Services.AddScoped<CreateLocationUseCase>();

var authApiBaseUrl = builder.Configuration["AuthApi:BaseUrl"];
if (string.IsNullOrWhiteSpace(authApiBaseUrl))
{
    throw new InvalidOperationException("AuthApi:BaseUrl não foi configurado. Ex.: http://localhost:5254");
}

builder.Services.AddHttpClient<IRequestAuth, RequestAuth>(client =>
{
    client.BaseAddress = new Uri(authApiBaseUrl);
});

builder.Services.AddAuthSecurity<object>(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.Run();