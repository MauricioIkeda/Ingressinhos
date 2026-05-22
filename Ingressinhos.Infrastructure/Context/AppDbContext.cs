using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Sales.Enums;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.Infrastructure.Context;

public class AppDbContext : DbContext
{
    private const string CatalogSchema = "catalog"; // Schema de "catalogo"
    private const string SalesSchema = "sales"; // Schama de "vendas"

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<Client> Clients { get; set; }
    public DbSet<IssuedTicket> IssuedTickets { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ComplexProperties<Price>();
        configurationBuilder.ComplexProperties<CPF>();
        configurationBuilder.ComplexProperties<CNPJ>();
        configurationBuilder.ComplexProperties<Money>();

        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>().UseTpcMappingStrategy(); // Falar que esse vai para os que herdam ele
        // Criando as tabelas nos schemas
        modelBuilder.Entity<Seller>().ToTable("Sellers", CatalogSchema);
        modelBuilder.Entity<Client>().ToTable("Clients", SalesSchema);
        modelBuilder.Entity<Event>().ToTable("Events", CatalogSchema);
        modelBuilder.Entity<Location>().ToTable("Locations", CatalogSchema);
        modelBuilder.Entity<Seat>().ToTable("Seats", CatalogSchema);
        modelBuilder.Entity<Ticket>().ToTable("Tickets", CatalogSchema);
        modelBuilder.Entity<Order>().ToTable("Orders", SalesSchema);
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems", SalesSchema);
        modelBuilder.Entity<IssuedTicket>().ToTable("IssuedTickets", SalesSchema);

        // Regras de mapeamento
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                email => email.Endereco,
                endereco => new Email(endereco))
            .IsRequired();

        modelBuilder.Entity<Seller>()
            .ComplexProperty(s => s.Cnpj, c => c.IsRequired());
                    
        modelBuilder.Entity<Client>()
            .ComplexProperty(client => client.Cpf, c => c.IsRequired());

        modelBuilder.Entity<Event>()
            .HasOne<Seller>()
            .WithMany()
            .HasForeignKey(eventEntity => eventEntity.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
            .HasOne<Location>()
            .WithMany()
            .HasForeignKey(eventEntity => eventEntity.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seat>()
            .HasOne<Location>()
            .WithMany()
            .HasForeignKey(seat => seat.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne<Event>()
            .WithMany()
            .HasForeignKey(ticket => ticket.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne<Seller>()
            .WithMany()
            .HasForeignKey(ticket => ticket.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(order => order.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .Navigation(order => order.Items)
            .AutoInclude();

        modelBuilder.Entity<Order>() // Deixando carrinho unico
            .HasIndex(order => order.ClientId)
            .HasFilter($"\"Status\" = {(int)OrderStatus.Cart}")
            .IsUnique();

        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany(order => order.Items)
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(orderItem => orderItem.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(orderItem => orderItem.SeatId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<IssuedTicket>()
            .HasOne<OrderItem>()
            .WithMany()
            .HasForeignKey(issuedTicket => issuedTicket.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IssuedTicket>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(issuedTicket => issuedTicket.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IssuedTicket>()
            .HasOne<Event>()
            .WithMany()
            .HasForeignKey(issuedTicket => issuedTicket.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
