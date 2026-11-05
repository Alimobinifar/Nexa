using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.GenericInterfaces
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void HardDelete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    }
}
