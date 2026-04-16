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
        configurationBuilder.ComplexProperties<Email>();
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
            .ComplexProperty(u => u.Email, c => c.IsRequired());

        modelBuilder.Entity<Seller>()
            .ComplexProperty(s => s.Cnpj, c => c.IsRequired());
                    
        modelBuilder.Entity<Client>()
            .ComplexProperty(c => c.Cpf, c => c.IsRequired()); // Supondo que o Client tenha CPF

        modelBuilder.Entity<PublishedTicket>()
            .ComplexProperty(t => t.UnitPrice, c => c.IsRequired());
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}