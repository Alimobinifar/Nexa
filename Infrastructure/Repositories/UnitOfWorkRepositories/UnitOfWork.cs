using Domain.Entities.BaseEntities;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Infrastructure.DataBase;
using Infrastructure.Repositories.GenericRepositories;
using Microsoft.EntityFrameworkCore.Storage;


namespace Infrastructure.Repositories.UnitOfWorkRepositories
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly TenantDbContext _context;

        private readonly Dictionary<Type, object> _repositories = new();

        private IDbContextTransaction? _transaction;

        private bool disposedValue;

        public UnitOfWork(TenantDbContext context)
        {
            _context = context;
        }

        // دریافت Repository متناسب با Entity
        public IGenericRepository<T> Repository<T>()
            where T : BaseModel
        {
            var type = typeof(T);

            if (!_repositories.TryGetValue(type, out var repository))
            {
                repository = new GenericRepository<T>(_context);
                _repositories.Add(type, repository);
            }

            return (IGenericRepository<T>)repository;
        }

        // ذخیره تغییرات
        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // شروع تراکنش
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Transaction already started.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        // تأیید تراکنش
        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction.");

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction.");

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
