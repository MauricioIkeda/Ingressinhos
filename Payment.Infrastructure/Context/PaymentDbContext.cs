using Generic.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Context;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<Refund> Refunds { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ComplexProperties<Price>();
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentTransaction>().ToTable("PaymentTransactions");
        modelBuilder.Entity<Refund>().ToTable("Refunds");

        modelBuilder.Entity<Refund>()
            .HasOne<PaymentTransaction>()
            .WithMany()
            .HasForeignKey(refund => refund.PaymentTransactionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
