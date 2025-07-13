using AutoMapper.QueryableExtensions;
using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Models;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;

namespace Ecommerce_brand_Api.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IBaseRepository<ProductImagesPaths> _imagesPaths;

      
        public ProductService(IUnitofwork unitOfWork, IMapper mapper, IWebHostEnvironment env , IBaseRepository<ProductImagesPaths> imagesPaths) : base(unitOfWork.GetBaseRepository<Product>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;   
            _env = env;
            _imagesPaths = imagesPaths;
        }

        public async Task<ServiceResult> AddProductAsync(ProductDtoRequest dto)
        {
            var product = _mapper.Map<Product>(dto);

            product.ProductSizes = new List<ProductSizes>();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (dto.Images == null || !dto.Images.Any(i => i != null && i.Length > 0))
            {
                return ServiceResult.Fail("At least one product image is required.");
            }

            for (int i = 0; i < dto.Images.Count; i++)
                {
                    var image = dto.Images[i];

                    if (image != null && image.Length > 0)
                    {
                        var extension = Path.GetExtension(image.FileName);
                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var fullPath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        product.ProductImagesPaths.Add(new ProductImagesPaths
                        {
                            ImagePath = $"/uploads/{fileName}".Replace("\\", "/"),
                            Priority = i + 1 
                        });
                    }
                }

            if (dto.ProductSizes != null && dto.ProductSizes.Any())
            {
                foreach (var sizeDto in dto.ProductSizes)
                {
                    if (!string.IsNullOrWhiteSpace(sizeDto.Size))
                    {
                        product.ProductSizes.Add(new ProductSizes
                        {
                            Size = sizeDto.Size,
                            Width =sizeDto.Width,   
                            Height =sizeDto.Height,
                            StockQuantity = sizeDto.StockQuantity
                        });
                    }
                }
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var productResponsedto = _mapper.Map<ProductDtoResponse>(product);

            return new ServiceResult
            {
                Success = true,
                SuccessMessage = "Product Added Successfully"
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<ProductDtoResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithImagesAsync();
            return _mapper.Map<IEnumerable<ProductDtoResponse>>(products);
        }


        public async Task<ProductDtoResponse?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(id);
            return product == null ? null : _mapper.Map<ProductDtoResponse>(product);
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

        public async Task<IEnumerable<ProductDtoResponse>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllWithImagesAsync();
                var filtered = products
                    .Where(p => p.CategoryId == categoryId && !p.IsDeleted);

                return _mapper.Map<IEnumerable<ProductDtoResponse>>(filtered);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while retrieving products by category.", ex);
            }
        }

        public async Task<ServiceResult> AddProductSizeToProductAsync(List<ProductSizesDto> dtoList)
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

        public async Task<bool> UpdateProductSizes(List<ProductSizesDto> productsSizesDto)
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

        public async Task<PaginatedResult<ProductDtoResponse>> GetPaginatedProductsAsync(ProductFilterParams filter)
        {
            var productRepo = _unitOfWork.GetBaseRepository<Product>();

            IQueryable<Product> query = productRepo.GetQueryable();

            // Include العلاقات
            query = query
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductImagesPaths);

            // فلترة حسب الكاتيجوري
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            // فلترة حسب الكلمة المفتاحية في الاسم أو الوصف
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var keyword = filter.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword));
            }

            // فلترة حسب السعر الأدنى
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            // فلترة حسب السعر الأقصى
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            // فلترة حسب المنتجات الجديدة (هتحتاج تكون معرف حاجة زي IsNewArrival أو CreatedAt قريب)
            if (filter.isNewArrival.HasValue && filter.isNewArrival.Value)
            {
                var recentDate = DateTime.UtcNow.AddDays(-7); // آخر 7 أيام مثلاً
                query = query.Where(p => p.CreatedAt >= recentDate);
            }

            // ترتيب حسب الأحدث
            query = query.OrderByDescending(p => p.CreatedAt);

            // تحويل لـ DTO وتطبيق البيچينيشن
            var pagedResult = await query
                .ProjectTo<ProductDtoResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(filter.PageIndex, filter.PageSize);

            return pagedResult;
        }

        public async Task<ProductDtoResponse> AddToNewArrivals(int Id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(Id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductDtoResponse>(product);
        }


        public async Task<bool> UpdateBasicInfoAsync(ProductBaseUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
            if (product == null)
                return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.DiscountPercentage = dto.DiscountPercentage;
            product.CategoryId = dto.CategoryId;
            product.IsDeleted = dto.IsDeleted;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReplaceImageByIdAsync(int imageId, IFormFile newImageFile)
        {
            var imageEntity = await _imagesPaths.GetByIdAsync(imageId);
            if (imageEntity == null) return false;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // حذف الصورة القديمة من wwwroot/uploads
            var oldFullPath = Path.Combine(_env.WebRootPath, imageEntity.ImagePath.TrimStart('/').Replace("/", "\\"));
            if (File.Exists(oldFullPath))
                File.Delete(oldFullPath);

            // تخزين الصورة الجديدة
            var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(newImageFile.FileName)}";
            var newFullPath = Path.Combine(uploadsFolder, newFileName);

            using var stream = new FileStream(newFullPath, FileMode.Create);
            await newImageFile.CopyToAsync(stream);

            // تحديث مسار الصورة فقط
            imageEntity.ImagePath = $"/uploads/{newFileName}".Replace("\\", "/");

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProductSizesAsync(ProductSizesUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null)
                return false;

            product.ProductSizes.Clear();

            foreach (var sizeDto in dto.ProductSizes)
            {
                if (!string.IsNullOrWhiteSpace(sizeDto.Size))
                {
                    product.ProductSizes.Add(new ProductSizes
                    {
                        Size = sizeDto.Size,
                        StockQuantity = sizeDto.StockQuantity
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }



    }
}
