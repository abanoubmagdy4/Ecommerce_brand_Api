namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IBaseService<T> where T : class
    {

        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Attach(T entity);
        void SoftDelete(T entity);
        Task<T> GetByStringIdAsync(string id);

    }
}
