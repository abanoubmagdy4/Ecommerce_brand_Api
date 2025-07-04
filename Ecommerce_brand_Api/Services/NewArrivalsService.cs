﻿using AutoMapper.QueryableExtensions;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services
{
    public class NewArrivalsService : INewArrivalsService
    {
        private readonly IUnitofwork _unitofwork;
        private readonly IMapper _mapper;

        public NewArrivalsService(IUnitofwork unitofwork, IMapper mapper)
        {
            this._unitofwork = unitofwork;
            this._mapper = mapper;
        }

        public async Task<ProductDto> AddNewArrivalAsync(int productId)
        {
            var product = await _unitofwork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            // Check if the product is already a new arrival
            var existingNewArrival = await _unitofwork.NewArrivals.IsProductNewArrivalAsync(productId);
            if (existingNewArrival)
            {
                throw new Exception("Product is already a new arrival");
            }
            var newArrival = new NewArrivals
            {
                ProductId = productId,
            };
            // Map the product to ProductDto
            var productDto = _mapper.Map<ProductDto>(product);
            await _unitofwork.NewArrivals.AddAsync(newArrival);
            await _unitofwork.SaveChangesAsync();
            return productDto;
        }

        public async Task<bool> DeleteNewArrival(int Id)
        {
            var product = await _unitofwork.NewArrivals.GetByIdAsync(Id);

            if (product == null)
            {
                throw new Exception("New Arrival not found");
            }
            //var productDto = mapper.Map<ProductDto>(product);
            await _unitofwork.NewArrivals.DeleteAsync(Id);
            await _unitofwork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<ProductDto>> GetNewArrivalsAsync(PaginationParams pagination)
        {
            var newArrivalsRepo = _unitofwork.GetBaseRepository<NewArrivals>();

            var query = newArrivalsRepo.GetQueryable()
                .Include(na => na.Product)
                .Select(na => na.Product)
                .OrderBy(p => p.Id);

            var pagedResult = await query
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(pagination.PageIndex, pagination.PageSize);

            return pagedResult;
        }
    }
}
