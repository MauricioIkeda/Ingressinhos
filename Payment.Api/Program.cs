using Microsoft.AspNetCore.OData;
using Payment.Api.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddOData(options => options.EnableQueryFeatures().AddRouteComponents("odata", ODataExtensions.GetPaymentEdmModel())); // Configura o OData para usar o modelo EDM definido em ODataExtensions
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Payment API", Version = "v1" });
});
builder.Services.AddPaymentServices(builder.Configuration, builder.Environment.ContentRootPath);

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
