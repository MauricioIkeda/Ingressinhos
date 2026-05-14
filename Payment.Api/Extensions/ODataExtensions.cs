using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Payment.Domain.Entities;

namespace Payment.Api.Extensions;

public static class ODataExtensions
{
    public static IEdmModel GetPaymentEdmModel()
    {
        var builder = new ODataConventionModelBuilder();

        ConfigureComplexTypes(builder);

        AddEntitySet<PaymentTransaction>(builder, "PaymentTransactions");
        AddEntitySet<Refund>(builder, "Refunds");

        builder.EntityType<PaymentTransaction>().ComplexProperty(payment => payment.Amount);

        return builder.GetEdmModel();
    }

    private static void ConfigureComplexTypes(ODataConventionModelBuilder builder)
    {
        var price = builder.ComplexType<Price>();
        price.DerivesFromNothing();
        price.Property(value => value.Value);
    }

    private static void AddEntitySet<TEntity>(ODataConventionModelBuilder builder, string name)
        where TEntity : BaseEntity
    {
        builder.EntitySet<TEntity>(name);
        builder.EntityType<TEntity>().HasKey(entity => entity.Id);
    }
}
