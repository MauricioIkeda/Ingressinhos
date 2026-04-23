using System.Linq.Expressions;
using Generic.Domain.Entities;


namespace Generic.Infrastructure.Interfaces
{
    public interface IRepositoryQuery
    {
        T Return<T>(long id) where T : BaseEntity;

        IQueryable<T> Query<T>() where T : BaseEntity;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> where) where T : BaseEntity;
        int Count<T>(Expression<Func<T, bool>> where) where T : BaseEntity;
        Task<T> ReturnAsync<T>(long id) where T : BaseEntity;
    }
}
