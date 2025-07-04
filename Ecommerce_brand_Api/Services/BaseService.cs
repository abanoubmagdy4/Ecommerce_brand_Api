
namespace Ecommerce_brand_Api.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IBaseRepository<T> _repository;
        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            return _repository.AddRangeAsync(entities);
        }

        public void Attach(T entity)
        {
            _repository.Attach(entity);
        }

        public Task DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<List<T>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<T> GetByStringIdAsync(string id)
        {
            return _repository.GetByStringIdAsync(id);
        }

        public void SoftDelete(T entity)
        {
            _repository.SoftDelete(entity);
        }

        public Task UpdateAsync(T entity)
        {
            return _repository.UpdateAsync(entity);
        }
    

    }
}
