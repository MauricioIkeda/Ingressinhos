using Generic.Domain.Entities;
using Generic.Domain.ValueObjects;
using Ingressinhos.Domain.Catalog.Entities;
using Ingressinhos.Domain.Payment.Entities;
using Ingressinhos.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<PublishedTicket> PublishedTickets { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<Refund> Refunds { get; set; }

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
        
        modelBuilder.Entity<User>().UseTpcMappingStrategy();
        modelBuilder.Entity<Seller>().ToTable("Sellers");
        modelBuilder.Entity<Client>().ToTable("Clients");

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

        modelBuilder.Entity<PublishedTicket>()
            .ComplexProperty(t => t.UnitPrice, c => c.IsRequired());

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

        modelBuilder.Entity<PublishedTicket>()
            .HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(publishedTicket => publishedTicket.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PublishedTicket>()
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(publishedTicket => publishedTicket.SeatId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(order => order.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(orderItem => orderItem.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(paymentTransaction => paymentTransaction.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Refund>()
            .HasOne<PaymentTransaction>()
            .WithMany()
            .HasForeignKey(refund => refund.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}