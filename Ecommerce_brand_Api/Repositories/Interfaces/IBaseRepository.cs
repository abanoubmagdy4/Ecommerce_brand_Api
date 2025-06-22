using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Attach(T entity);
        void SoftDelete(T entity);
        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null
        );
    }
}
