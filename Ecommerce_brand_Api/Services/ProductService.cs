using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;


        public ProductService(IUnitofwork unitOfWork, IMapper mapper, IWebHostEnvironment env)
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
            var products = await _unitOfWork.Products.GetAllWithImagesAsync(); // Ensure Include Images in repo
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }


        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(id); // Ensure Include Images
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }


        public async Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdWithImagesAsync(id);
            if (product == null) return false;

            _mapper.Map(dto, product);

            // Optional: remove old images if needed (file system and DB)
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


        public async Task<bool> DecreaseStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.IsDeleted)
                    return false;

                //if (product.StockQuantity < quantity)
                //    throw new InvalidOperationException("Insufficient stock.");

                //product.StockQuantity -= quantity;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Error while decreasing product stock.", ex);
            }
        }
        public async Task<bool> IncreaseStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.IsDeleted)
                    return false;

                //product.StockQuantity += quantity;
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error while increasing product stock.", ex);
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
    }
}
