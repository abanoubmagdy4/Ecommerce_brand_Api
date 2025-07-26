using AutoMapper.QueryableExtensions;
using Ecommerce_brand_Api.Helpers.BackgroundServices;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;
using Hangfire;

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

        public async Task<ProductDtoResponse> AddNewArrivalAsync(int productId)
        {
            var product = await _unitofwork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            // Check if the product is already a new arrival BEFORE updating anything
            var existingNewArrival = await _unitofwork.NewArrivals.IsProductNewArrivalAsync(productId);
            if (existingNewArrival)
            {
                throw new Exception("Product is already a new arrival");
            }

            // Now mark as new arrival
            product.isNewArrival = true;
            await _unitofwork.Products.UpdateAsync(product);

            var newArrival = new NewArrivals
            {
                ProductId = productId,
            };

            await _unitofwork.NewArrivals.AddAsync(newArrival);
            await _unitofwork.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDtoResponse>(product);

            if (productDto.PublishAt.HasValue)
            {
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                productDto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productDto.PublishAt.Value, egyptTimeZone);
            }

            var delay = product.PublishAt - DateTime.UtcNow;
            if (delay < TimeSpan.Zero)
                delay = TimeSpan.Zero;

            BackgroundJob.Schedule<ProductPublisherJob>(
                job => job.PublishProduct(product.Id),
                delay.Value);

            return productDto;
        }


        public async Task<bool> DeleteNewArrival(int Id)
        {
            var product = await _unitofwork.NewArrivals.GetNewArrivalByProductId(Id);

            if (product == null)
            {
                throw new Exception("New Arrival not found");
            }
            product.isNewArrival = false;
            await _unitofwork.Products.UpdateAsync(product);
            //var productDto = mapper.Map<ProductDto>(product);
            await _unitofwork.NewArrivals.DeleteProductFromAsync(Id);
            await _unitofwork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<ProductDtoResponse>> GetNewArrivalsAsync(PaginationParams pagination)
        {
            var newArrivalsRepo = _unitofwork.GetBaseRepository<NewArrivals>();

            var query = newArrivalsRepo.GetQueryable()
                .Include(na => na.Product)
                .Where(na => na.Product != null
                             && !na.Product.IsDeleted
                             && na.Product.IsPublished)
                .Select(na => na.Product)
                .OrderByDescending(p => p.CreatedAt);

            var pagedResult = await query
                .ProjectTo<ProductDtoResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(pagination.PageIndex, pagination.PageSize);

            // تحويل PublishAt لتوقيت مصر
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            foreach (var productDto in pagedResult.Items)
            {
                if (productDto.PublishAt.HasValue)
                {
                    productDto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productDto.PublishAt.Value, egyptTimeZone);
                }
            }

            return pagedResult;
        }


    }
}

