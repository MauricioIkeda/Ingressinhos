using Generic.Application.Interface;
using Generic.Infrastructure.Interfaces;
using Generic.Infrastructure.Repositories;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Catalog.UseCases;
using Ingressinhos.Domain.Catalog.Entities;
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
builder.Services.AddScoped<SellerInclude>();
builder.Services.AddScoped<SellerUpdate>();
builder.Services.AddScoped<IUseCaseCrudCollection<Seller, SellerDto>, UseCaseSellerCollection>();

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
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.Run();