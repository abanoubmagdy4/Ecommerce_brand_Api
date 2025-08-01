﻿using AutoMapper.QueryableExtensions;
using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.BackgroundServices;
using Ecommerce_brand_Api.Models;
using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities.Pagination;
using Hangfire;

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
            var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var localDateTime = dto.PublishAt; 
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, egyptZone);

            product.PublishAt = utcDateTime;

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var delay = product.PublishAt - DateTime.UtcNow;

            if (delay < TimeSpan.Zero)
                delay = TimeSpan.Zero;

            BackgroundJob.Schedule<ProductPublisherJob>(
                job => job.PublishProduct(product.Id),
                delay.Value);

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

        public async Task<bool> RestoreProduct(int id)
        {
            var product = await _unitOfWork.Products.GetProductById(id);
            if (product == null) return false;

            product.IsDeleted = false;
            await _unitOfWork.SaveChangesAsync();

  
            return true;
        }


        public async Task<IEnumerable<ProductDtoResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithImagesAsync();

            var productDtos = _mapper.Map<IEnumerable<ProductDtoResponse>>(products);

            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            foreach (var productDto in productDtos)
            {
                if (productDto.PublishAt.HasValue)
                {
                    productDto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productDto.PublishAt.Value, egyptTimeZone);
                }
            }

            return productDtos;
        }



        public async Task<ProductDtoResponse?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(id);
            if (product == null)
                return null;

            var productDto = _mapper.Map<ProductDtoResponse>(product);

            if (productDto.PublishAt.HasValue)
            {
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                productDto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productDto.PublishAt.Value, egyptTimeZone);
            }

            return productDto;
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

                var productDtos = _mapper.Map<IEnumerable<ProductDtoResponse>>(filtered);

                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                foreach (var productDto in productDtos)
                {
                    if (productDto.PublishAt.HasValue)
                    {
                        productDto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productDto.PublishAt.Value, egyptTimeZone);
                    }
                }

                return productDtos;
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

        public async Task<List<ProductSizesDto>> UpdateProductSizes(List<ProductSizesDto> productsSizesDto)
        {
            var ids = productsSizesDto.Select(x => x.Id).ToList();

            var existingSizes = await _unitOfWork.ProductsSizes
                .GetQueryable()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            var existingDict = existingSizes.ToDictionary(x => x.Id);
            var updatedDtos = new List<ProductSizesDto>();

            foreach (var dto in productsSizesDto)
            {
                if (!existingDict.TryGetValue(dto.Id, out var existing))
                    continue; // أو throw حسب المطلوب

                _mapper.Map(dto, existing);
                updatedDtos.Add(_mapper.Map<ProductSizesDto>(existing));
            }

            await _unitOfWork.SaveChangesAsync();

            return productsSizesDto;
        }



        public async Task<PaginatedResult<ProductDtoResponse>> GetPaginatedProductsForCustomerAsync(ProductFilterParams filter)
        {
            var productRepo = _unitOfWork.GetBaseRepository<Product>();
            IQueryable<Product> query = productRepo.GetQueryable();

            // Include العلاقات
            query = query   
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductImagesPaths).Where(p=>p.IsPublished);

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

            // تنفيذ الاستعلام وتحويل لـ DTO
            var pagedResult = await query
                .ProjectTo<ProductDtoResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(filter.PageIndex, filter.PageSize);

            // تحويل PublishAt لكل منتج لتوقيت مصر
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            foreach (var item in pagedResult.Items)
            {
                if (item.PublishAt.HasValue)
                {
                    item.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(item.PublishAt.Value, egyptTimeZone);
                }
            }

            return pagedResult;
        }

        public async Task<PaginatedResult<ProductDtoResponse>> GetPaginatedProductsForAdminDashboardAsync(ProductFilterParams filter)
        {
            var query = _unitOfWork.Products.GetAllProductsForAdminDashboardQueryable();
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

            // تنفيذ الاستعلام وتحويل لـ DTO
            var pagedResult = await query
                .ProjectTo<ProductDtoResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(filter.PageIndex, filter.PageSize);

            // تحويل PublishAt لكل منتج لتوقيت مصر
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            foreach (var item in pagedResult.Items)
            {
                if (item.PublishAt.HasValue)
                {
                    item.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(item.PublishAt.Value, egyptTimeZone);
                }
            }

            return pagedResult;
        }
        public async Task<PaginatedResult<ProductDtoResponse>> GetPaginatedDeletedProductsAsync(ProductFilterParams filter)
        {
            var query = _unitOfWork.Products.GetAllDeletedProductsQueryable();    

        
       

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

            // تنفيذ الاستعلام وتحويل لـ DTO
            var pagedResult = await query
                .ProjectTo<ProductDtoResponse>(_mapper.ConfigurationProvider)
                .ToPaginatedResultAsync(filter.PageIndex, filter.PageSize);

            // تحويل PublishAt لكل منتج لتوقيت مصر
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            foreach (var item in pagedResult.Items)
            {
                if (item.PublishAt.HasValue)
                {
                    item.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(item.PublishAt.Value, egyptTimeZone);
                }
            }

            return pagedResult;
        }

        public async Task<ProductDtoResponse> AddToNewArrivals(int Id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(Id);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            await _unitOfWork.SaveChangesAsync();

            var productDto = _mapper.Map<ProductDtoResponse>(product);

            if (productDto.PublishAt.HasValue)
            {
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                productDto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productDto.PublishAt.Value, egyptTimeZone);
            }

            return productDto;
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
            product.IsPublished = dto.IsPublished;
            product.PublishAt= dto.PublishAt;

            await _unitOfWork.SaveChangesAsync();

            var delay = product.PublishAt - DateTime.UtcNow;

            if (delay < TimeSpan.Zero)
                delay = TimeSpan.Zero;

            BackgroundJob.Schedule<ProductPublisherJob>(
                job => job.PublishProduct(product.Id),
                delay.Value);

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
