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
    }
}
