﻿using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface INewArrivalsService
    {
        public Task<ProductDto> AddNewArrivalAsync(int Id);
        public Task<PaginatedResult<ProductDto>> GetNewArrivalsAsync(PaginationParams pagination);
        public Task<bool> DeleteNewArrival(int Id);
    }
}
