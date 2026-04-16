
namespace Generic.Infrastructure.Interfaces
{
    public interface IRepositorySession 
    {
        IRepositoryQuery GetRepositoryQuery();

        IRepository GetRepository();

        IDisposable BeginTransaction();
        void CommitTransaction();

        void RollbackTransaction();

        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
