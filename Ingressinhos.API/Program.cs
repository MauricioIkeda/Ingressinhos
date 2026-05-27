using Generic.Api.Extensions;
using Ingressinhos.API.Extensions;
using Microsoft.AspNetCore.OData;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.AddSlugifiedRoutes();
})
.AddOData(options => options.EnableQueryFeatures().AddRouteComponents("odata", ODataExtensions.GetIngressinhosEdmModel())); // configura o OData

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("catalog", new() { Title = "Ingressinhos API - Catalog", Version = "v1" });
    c.SwaggerDoc("sales", new() { Title = "Ingressinhos API - Sales", Version = "v1" });
    c.DocInclusionPredicate((documentName, apiDescription) =>
    {
        var groupName = apiDescription.GroupName;
        return !string.IsNullOrWhiteSpace(groupName)
            && string.Equals(groupName, documentName, StringComparison.OrdinalIgnoreCase);
    });
});

builder.Services.AddIngressinhosServices(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddAuthSecurity<object>(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
        options.AddDocuments(["catalog", "sales"]);
    });
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/scalar/catalog"));

app.Run();
