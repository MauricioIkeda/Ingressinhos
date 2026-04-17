using System.Linq.Expressions;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Generic.Infrastructure.Repositories;

public class RepositoryQueryEF : IRepositoryQuery
{
    private readonly DbContext _context;

    public RepositoryQueryEF(DbContext context)
    {
        _context = context;
    }

    public T Return<T>(long id) where T : BaseEntity
    {
        return _context.Set<T>().FirstOrDefault(entity => entity.Id == id);
    }

    public IQueryable<T> Query<T>() where T : BaseEntity
    {
        return _context.Set<T>();
    }

    public IQueryable<T> Query<T>(Expression<Func<T, bool>> where) where T : BaseEntity
    {
        return _context.Set<T>().Where(where);
    }

    public Task<T> ReturnAsync<T>(long id) where T : BaseEntity
    {
        return _context.Set<T>().FirstOrDefaultAsync(entity => entity.Id == id);
    }
}
