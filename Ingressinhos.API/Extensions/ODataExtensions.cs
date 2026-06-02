using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Application.Sales.Dtos;
using Ingressinhos.Application.Sales.TicketReadModel.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Ingressinhos.API.Extensions;

public static class ODataExtensions // Isso daqui serve para configurar os modelos Odatas, para ele saber mapear
{
    public static IEdmModel GetIngressinhosEdmModel()
    {
        var builder = new ODataConventionModelBuilder();

        ConfigureComplexTypes(builder);

        AddEntitySet<Event>(builder, "Events");
        AddEntitySet<Location>(builder, "Locations");
        AddEntitySet<Seat>(builder, "Seats");
        AddEntitySet<Ticket>(builder, "Tickets");
        AddEntitySet<Seller>(builder, "Sellers");
        AddEntitySet<Client>(builder, "Clients");
        AddEntitySet<Order>(builder, "Orders");
        AddEntitySet<OrderItem>(builder, "OrderItems");
        AddEntitySet<IssuedTicket>(builder, "IssuedTickets");

        builder.EntityType<Client>().ComplexProperty(client => client.Cpf);
        builder.EntityType<Client>().ComplexProperty(client => client.Email);
        builder.EntityType<Seller>().ComplexProperty(seller => seller.Cnpj);
        builder.EntityType<Seller>().ComplexProperty(seller => seller.Email);
        builder.EntityType<Ticket>().ComplexProperty(ticket => ticket.BasePrice);
        builder.EntityType<Ticket>().ComplexProperty(ticket => ticket.PremiumPrice);
        builder.EntityType<Ticket>().ComplexProperty(ticket => ticket.VIPPrice);
        builder.EntityType<OrderItem>().ComplexProperty(orderItem => orderItem.UnitPrice);

        return builder.GetEdmModel();
    }

    public static IEdmModel GetClientQueryEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<ClientQueryItem>("Clients");
        builder.EntityType<ClientQueryItem>().HasKey(client => client.Id);
        return builder.GetEdmModel();
    }

    public static IEdmModel GetSellerQueryEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<SellerQueryItem>("Sellers");
        builder.EntityType<SellerQueryItem>().HasKey(seller => seller.Id);
        return builder.GetEdmModel();
    }

    public static IEdmModel GetTicketQueryEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<TicketQueryItem>("Tickets");
        builder.EntityType<TicketQueryItem>().HasKey(ticket => ticket.Id);
        return builder.GetEdmModel();
    }

    public static IEdmModel GetEventWithTicketsQueryEdmModel() // modelo de Dto paa evento e ticket
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<EventWithTicketsDto>("Events");
        builder.EntityType<EventWithTicketsDto>().HasKey(eventItem => eventItem.Id);
        builder.ComplexType<EventTicketWithPricesDto>();
        builder.EntityType<EventWithTicketsDto>().CollectionProperty(eventItem => eventItem.Tickets);
        return builder.GetEdmModel();
    }

    public static IEdmModel GetOrderItemQueryEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<OrderItemQueryItem>("OrderItems");
        builder.EntityType<OrderItemQueryItem>().HasKey(orderItem => orderItem.Id);
        return builder.GetEdmModel();
    }

    public static IEdmModel GetClientTicketViewEdmModel() // modelo de Dto para a view de ingressos do cliente
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<ClientTicketViewDto>("IssuedTickets");
        builder.EntityType<ClientTicketViewDto>().HasKey(ticket => ticket.IssuedTicketId);
        return builder.GetEdmModel();
    }

    private static void ConfigureComplexTypes(ODataConventionModelBuilder builder)
    {
        var cpf = builder.ComplexType<CPF>();
        cpf.DerivesFromNothing();
        cpf.Property(value => value.Numero);

        var cnpj = builder.ComplexType<CNPJ>();
        cnpj.DerivesFromNothing();
        cnpj.Property(value => value.Numero);

        var email = builder.ComplexType<Email>();
        email.DerivesFromNothing();
        email.Property(value => value.Endereco);

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
