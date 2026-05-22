using Microsoft.EntityFrameworkCore;

namespace Ingressinhos.Infrastructure.Context;

// Contexto dedicado a leitura da replica. Nao gere migrations nem rode Update-Database por ele.
public class ReadAppDbContext : AppDbContext
{
    public ReadAppDbContext(DbContextOptions<ReadAppDbContext> options) : base(options)
    {
    }
}
