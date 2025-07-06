using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface INewArrivalsService
    {
        Task<ProductDtoResponse> AddNewArrivalAsync(int productId);
        public Task<PaginatedResult<ProductDtoResponse>> GetNewArrivalsAsync(PaginationParams pagination);
        public Task<bool> DeleteNewArrival(int Id);
    }
}
