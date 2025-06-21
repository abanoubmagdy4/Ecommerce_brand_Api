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

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
