using System.Linq.Expressions;
using System.Reflection;
using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Generic.Infrastructure.Repositories;

public class RepositoryEF : IRepository
{
    private readonly DbContext _context;

    public RepositoryEF(DbContext context)
    {
        _context = context;
    }

    public void Include<T>(T entity) where T : BaseEntity
    {
        _context.Set<T>().Add(entity);
    }

    public void Upsert<T>(T entity) where T : BaseEntity
    {
        if (entity.Id <= 0)
        {
            _context.Set<T>().Add(entity);
            return;
        }

        _context.Set<T>().Update(entity);
    }

    public T Merge<T>(T entity) where T : BaseEntity
    {
        Upsert(entity);
        return entity;
    }

    public void Delete<T>(T entity) where T : BaseEntity
    {
        _context.Set<T>().Remove(entity);
    }

    public Task IncludeAsync<T>(T entity) where T : BaseEntity
    {
        return _context.Set<T>().AddAsync(entity).AsTask();
    }

    public Task FlushAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task Flush()
    {
        return _context.SaveChangesAsync();
    }

    public Task UpsertAsync<T>(T entity) where T : BaseEntity
    {
        Upsert(entity);
        return Task.CompletedTask;
    }

    public Task<T> MergeAsync<T>(T entity) where T : BaseEntity
    {
        return Task.FromResult(Merge(entity));
    }

    public Task DeleteAsync<T>(T entity) where T : BaseEntity
    {
        Delete(entity);
        return Task.CompletedTask;
    }

    public int Update<T>(Func<IBuilderUpdate<T>, IQueryable<T>> setValues) where T : BaseEntity
    {
        var updateBuilder = new BuilderUpdate<T>(_context, _context.Set<T>());
        _ = setValues(updateBuilder);
        return updateBuilder.ApplyUpdates();
    }

    public Task<int> UpdateAsync<T>(Func<IBuilderUpdate<T>, IQueryable<T>> setValues) where T : BaseEntity
    {
        var updateBuilder = new BuilderUpdate<T>(_context, _context.Set<T>());
        _ = setValues(updateBuilder);
        return updateBuilder.ApplyUpdatesAsync();
    }

    private sealed class BuilderUpdate<T> : IBuilderUpdate<T>
        where T : BaseEntity
    {
        private readonly DbContext _context;
        private readonly List<Action<T>> _setters = new();
        private IQueryable<T> _query;

        public BuilderUpdate(DbContext context, IQueryable<T> source)
        {
            _context = context;
            _query = source;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> exp)
        {
            _query = _query.Where(exp);
            return _query;
        }

        public IBuilderUpdate<T> Set<T2>(Expression<Func<T, T2>> prop, Expression<Func<T, T2>> value)
        {
            var propertyName = GetPropertyName(prop);
            var valueResolver = value.Compile();

            _setters.Add(entity =>
            {
                _context.Entry(entity).Property(propertyName).CurrentValue = valueResolver(entity);
            });

            return this;
        }

        public int ApplyUpdates()
        {
            if (_setters.Count == 0)
            {
                return 0;
            }

            var entities = _query.ToList();

            foreach (var entity in entities)
            {
                foreach (var setter in _setters)
                {
                    setter(entity);
                }
            }

            return entities.Count;
        }

        public async Task<int> ApplyUpdatesAsync()
        {
            if (_setters.Count == 0)
            {
                return 0;
            }

            var entities = await _query.ToListAsync();

            foreach (var entity in entities)
            {
                foreach (var setter in _setters)
                {
                    setter(entity);
                }
            }

            return entities.Count;
        }

        private static string GetPropertyName<T2>(Expression<Func<T, T2>> prop)
        {
            if (prop.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException("A propriedade do Set deve ser um acesso direto do tipo x => x.Propriedade.", nameof(prop));
            }

            if (memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("A expressao informada no Set deve apontar para uma propriedade.", nameof(prop));
            }

            return propertyInfo.Name;
        }
    }
}