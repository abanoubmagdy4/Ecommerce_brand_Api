﻿
namespace Ecommerce_brand_Api.Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> GetByIdWithImagesAsync(int id);
        Task<IEnumerable<Product>> GetAllWithImagesAsync();
        IQueryable<Product> GetAllDeletedProductsQueryable();
        Task<Product?> GetProductById(int id);
        IQueryable<Product> GetAllProductsForAdminDashboardQueryable();
    }
}
