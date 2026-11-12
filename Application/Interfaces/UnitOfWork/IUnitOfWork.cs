using Domain.Entities.BaseEntities;
using Domain.Interfaces.GenericInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseModel; 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
