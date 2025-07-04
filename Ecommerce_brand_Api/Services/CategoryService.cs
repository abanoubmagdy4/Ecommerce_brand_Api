using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Entities;
using Ecommerce_brand_Api.Repositories;

namespace Ecommerce_brand_Api.Services
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitofwork unitOfWork, IMapper mapper) : base(unitOfWork.GetBaseRepository<Category>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(CategoryDto dto)
        {
            var Ecategory = _mapper.Map<Category>(dto);
            await _unitOfWork.Categories.AddAsync(Ecategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            _unitOfWork.Categories.SoftDelete(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category is null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateAsync(int id, CategoryDto dto)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existing == null) return false;

            _mapper.Map(dto, existing);
            await _unitOfWork.Categories.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
