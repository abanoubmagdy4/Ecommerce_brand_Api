using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Repositories.Interfaces;

namespace Ecommerce_brand_Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitofwork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(CategoryDTO dto)
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

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category is null ? null : _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> UpdateAsync(int id, CategoryDTO dto)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existing == null) return false;

            _mapper.Map(dto, existing);
            _unitOfWork.Categories.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
