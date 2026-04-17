using Generic.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Generic.Infrastructure.Repositories;

public class RepositorySessionEF : IRepositorySession
{
    private readonly DbContext _context;
    private readonly IRepository _repository;
    private readonly IRepositoryQuery _repositoryQuery;

    public RepositorySessionEF(DbContext context)
    {
        _context = context;
        _repository = new RepositoryEF(context);
        _repositoryQuery = new RepositoryQueryEF(context);
    }

    public IRepositoryQuery GetRepositoryQuery()
    {
        return _repositoryQuery;
    }

    public IRepository GetRepository()
    {
        return _repository;
    }

    public IDisposable BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _context.Database.CurrentTransaction?.Commit();
    }

    public void RollbackTransaction()
    {
        _context.Database.CurrentTransaction?.Rollback();
    }

    public Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction is null)
        {
            return Task.CompletedTask;
        }

        return _context.Database.CurrentTransaction.CommitAsync();
    }

    public Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction is null)
        {
            return Task.CompletedTask;
        }

        return _context.Database.CurrentTransaction.RollbackAsync();
    }
}
