using System.Linq.Expressions;
using Generic.Domain.Entities;

namespace Generic.Infrastructure.Interfaces
{
    public interface IRepository
    {
        void Include<T>(T entity) where T : BaseEntity;
        void Upsert<T>(T entity) where T : BaseEntity;
        T Merge<T>(T entity) where T : BaseEntity;
        void Delete<T>(T entity) where T : BaseEntity;
        Task IncludeAsync<T>(T entity) where T : BaseEntity;
        Task FlushAsync();
        Task Flush();
        Task UpsertAsync<T>(T entity) where T : BaseEntity;
        Task<T> MergeAsync<T>(T entity) where T : BaseEntity;
        Task DeleteAsync<T>(T entity) where T : BaseEntity;
        int Update<T>(Func<IBuilderUpdate<T>, IQueryable<T>> setValues) where T : BaseEntity;
        Task<int> UpdateAsync<T>(Func<IBuilderUpdate<T>, IQueryable<T>> setValues) where T : BaseEntity;
    }

    public interface IBuilderUpdate<T>
    {
        IQueryable<T> Where(Expression<Func<T, bool>> exp);
        IBuilderUpdate<T> Set<T2>(Expression<Func<T, T2>> prop, Expression<Func<T, T2>> value);
    }
}

