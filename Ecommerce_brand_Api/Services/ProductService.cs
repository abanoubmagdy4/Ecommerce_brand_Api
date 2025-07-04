using AutoMapper.QueryableExtensions;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services
{
    public class ProductService :BaseService<Product> ,IProductService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;


        public ProductService(IUnitofwork unitOfWork, IMapper mapper, IWebHostEnvironment env): base(unitOfWork.GetBaseRepository<Product>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task AddAsync(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.ProductImagesPaths = new List<ProductImagesPaths>();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in dto.Images)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                product.ProductImagesPaths.Add(new ProductImagesPaths
                {
                    ImagePath = Path.Combine("uploads", fileName).Replace("\\", "/")
                });
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithImagesAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }


        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }


        public async Task<bool> UpdateAsync(ProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(dto.Id);
            if (product == null) return false;

            _mapper.Map(dto, product);

            if (dto.Images != null && dto.Images.Any())
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                foreach (var file in dto.Images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    product.ProductImagesPaths.Add(new ProductImagesPaths
                    {
                        ImagePath = Path.Combine("uploads", fileName).Replace("\\", "/")
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DecreaseStockAsync(int productSizeId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.ProductsSizes.GetByIdAsync(productSizeId);
                if (product == null)
                    return false;

                if (product.StockQuantity < quantity)
                    throw new InvalidOperationException("Insufficient stock.");

                product.StockQuantity -= quantity;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Error while decreasing product stock.", ex);
            }
        }

        public async Task<bool> IncreaseStockAsync(int productSizeId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.ProductsSizes.GetByIdAsync(productSizeId);
                if (product == null)
                    return false;

                product.StockQuantity += quantity;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Error while decreasing product stock.", ex);
            }
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllWithImagesAsync();
                var filtered = products
                    .Where(p => p.CategoryId == categoryId && !p.IsDeleted);

                return _mapper.Map<IEnumerable<ProductDto>>(filtered);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while retrieving products by category.", ex);
            }
        }

        public async Task<ServiceResult> AddProductSizeToProductAsync(List<ProductSizeDto> dtoList)
        {
            try
            {
                foreach (var dto in dtoList)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException("Product not found.");
                    var productSize = _mapper.Map<ProductSizes>(dto);
                    productSize.ProductId = dto.ProductId;
                    await _unitOfWork.ProductsSizes.AddAsync(productSize);
                }
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Ok("Product size added successfully.");

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while adding product size.", ex);
            }
        }

        public async Task<bool> UpdateProductSizes(List<ProductSizeDto> productsSizesDto)
        {
            try
            {
                foreach (var dto in productsSizesDto)
                {
                    var existingProductSize = await _unitOfWork.ProductsSizes.GetByIdAsync(dto.Id);
                    if (existingProductSize == null)
                        throw new KeyNotFoundException($"Product size with ID {dto.Id} not found.");

                    _mapper.Map(dto, existingProductSize);

                    await _unitOfWork.ProductsSizes.UpdateAsync(existingProductSize);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while updating product sizes.", ex);
            }
        }

        public async Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(PaginationParams pagination)
        {
            var productRepo = _unitOfWork.GetBaseRepository<Product>();

            var query = productRepo.GetQueryable().OrderBy(p => p.Id);

            var pagedResult = await query
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(pagination.PageIndex, pagination.PageSize);

            return pagedResult;
        }

        public async Task<ProductDto> AddToNewArrivals(int Id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(Id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }


    }
}
