using Auth.Domain.Entities;
using Generic.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Context;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<UserAuth> UserAuths { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserAuth>().ToTable("AuthUsers");

        modelBuilder.Entity<UserAuth>()
            .Property(u => u.Email)
            .HasConversion(
                email => email.Endereco,
                endereco => new Email(endereco))
            .IsRequired();


        modelBuilder.Entity<UserAuth>()
            .HasIndex(x => x.UserId)
            .IsUnique();
    }
}