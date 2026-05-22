using Generic.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Generic.Infrastructure.Repositories;

public class ReadRepositoryQueryEF : RepositoryQueryEF, IReadRepositoryQuery
{
    public ReadRepositoryQueryEF(DbContext context) : base(context)
    {
    }
}
