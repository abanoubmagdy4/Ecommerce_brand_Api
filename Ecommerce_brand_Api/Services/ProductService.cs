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


        public ProductService(IUnitofwork unitOfWork, IMapper mapper, IWebHostEnvironment env) : base(unitOfWork.GetBaseRepository<Product>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _env = env;
        }

        public async Task AddAsync(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.ProductImagesPaths = new List<ProductImagesPaths>();
            product.ProductSizes = new List<ProductSizes>();

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // تأكيد إن الصور مش null ولا فاضية
            if (dto.ProductImagesPaths != null && dto.ProductImagesPaths.Any())
            {
                foreach (var imageDto in dto.ProductImagesPaths)
                {
                    if (imageDto.File != null && imageDto.File.Length > 0)
                    {
                        var extension = Path.GetExtension(imageDto.File.FileName);
                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var fullPath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await imageDto.File.CopyToAsync(stream);
                        }

                        product.ProductImagesPaths.Add(new ProductImagesPaths
                        {
                            ImagePath = $"/uploads/{fileName}".Replace("\\", "/"),
                            Priority = imageDto.Priority
                        });
                    }
                    else if (!string.IsNullOrEmpty(imageDto.ImagePath))
                    {
                        product.ProductImagesPaths.Add(new ProductImagesPaths
                        {
                            ImagePath = imageDto.ImagePath,
                            Priority = imageDto.Priority
                        });
                    }
                }
            }

            // تأكيد إن السايزات مش null ولا فاضية
            if (dto.ProductSizes != null && dto.ProductSizes.Any())
            {
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
            if (product == null)
                return false;

            // تحديث الخصائص العامة
            _mapper.Map(dto, product);

            // مسح الصور القديمة من السيرفر
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            foreach (var oldImage in product.ProductImagesPaths)
            {
                var fullPath = Path.Combine(_env.WebRootPath, oldImage.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            // مسح الصور القديمة من قاعدة البيانات
            product.ProductImagesPaths.Clear();

            // إضافة الصور الجديدة
            foreach (var imageDto in dto.ProductImagesPaths)
            {
                if (imageDto.File != null && imageDto.File.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageDto.File.FileName);
                    var fullPath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await imageDto.File.CopyToAsync(stream);

                    product.ProductImagesPaths.Add(new ProductImagesPaths
                    {
                        ImagePath = Path.Combine("uploads", fileName).Replace("\\", "/"),
                        Priority = imageDto.Priority
                    });
                }
                else if (!string.IsNullOrEmpty(imageDto.ImagePath))
                {
                    // احتياطي: لو جاي صورة قديمة مش هيمسحها (ده لو عايز تحتفظ ببعض الصور)
                    product.ProductImagesPaths.Add(new ProductImagesPaths
                    {
                        ImagePath = imageDto.ImagePath,
                        Priority = imageDto.Priority
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

        public async Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(ProductFilterParams filter)
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
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(filter.PageIndex, filter.PageSize);

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

        public async Task<bool> UpdateProductImagesAsync(ProductImagesUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(dto.ProductId);
            if (product == null) return false;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // IDs بتاعت الصور اللي جت مع الريكوست (ممكن تبقى جديدة أو قديمة)
            var incomingImagePaths = dto.ProductImagesPaths
                .Where(i => string.IsNullOrEmpty(i.ImagePath) == false)
                .Select(i => i.ImagePath.Trim().ToLower())
                .ToList();

            // حذف الصور اللي مش موجودة في الريكوست (اتشالت من اليوزر)
            var imagesToRemove = product.ProductImagesPaths
                .Where(existing => !incomingImagePaths.Contains(existing.ImagePath.Trim().ToLower()))
                .ToList();

            foreach (var img in imagesToRemove)
            {
                var fullPath = Path.Combine(_env.WebRootPath, img.ImagePath.TrimStart('/').Replace("/", "\\"));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                product.ProductImagesPaths.Remove(img);
            }

            // إضافة الصور الجديدة (اللي معاها ملف)
            foreach (var imageDto in dto.ProductImagesPaths)
            {
                if (imageDto.File != null && imageDto.File.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageDto.File.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await imageDto.File.CopyToAsync(stream);

                    product.ProductImagesPaths.Add(new ProductImagesPaths
                    {
                        ImagePath = $"/uploads/{fileName}".Replace("\\", "/"),
                        Priority = imageDto.Priority
                    });
                }
                else
                {
                    // صورة قديمة موجودة مسبقًا → ممكن تعدل الـ Priority
                    var existingImage = product.ProductImagesPaths
                        .FirstOrDefault(p => p.ImagePath.Trim().ToLower() == imageDto.ImagePath.Trim().ToLower());

                    if (existingImage != null)
                    {
                        existingImage.Priority = imageDto.Priority;
                    }
                }
            }

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
