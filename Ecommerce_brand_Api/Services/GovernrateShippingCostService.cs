using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services
{
    public class GovernrateShippingCostService : BaseService<GovernorateShippingCost>, IGovernrateShippingCostService
    {
        private readonly IUnitofwork _unitOfWork;
        private readonly IMapper _mapper;

        public GovernrateShippingCostService(IUnitofwork unitOfWork, IMapper mapper) : base(unitOfWork.GetBaseRepository<GovernorateShippingCost>())
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GovernrateShippingCostDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.GovernratesShippingCosts.GetAllAsync();
            return _mapper.Map<IEnumerable<GovernrateShippingCostDto>>(entities);
        }

        public async Task<GovernrateShippingCostDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.GovernratesShippingCosts.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<GovernrateShippingCostDto>(entity);
        }

        public async Task<GovernrateShippingCostDto?> GetByNameAsync(string name)
        {
            var entity = await _unitOfWork.GovernratesShippingCosts.GetByNameAsync(name);
            return entity == null ? null : _mapper.Map<GovernrateShippingCostDto>(entity);
        }

        public async Task AddAsync(GovernrateShippingCostDto dto)
        {
            var entity = _mapper.Map<GovernorateShippingCost>(dto);
            await _unitOfWork.GovernratesShippingCosts.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, GovernrateShippingCostDto dto)
        {
            var entity = await _unitOfWork.GovernratesShippingCosts.GetByIdAsync(id);
            if (entity == null)
                return false;

            _mapper.Map(dto, entity); // ده بيعمل map للخصائص اللي اتغيرت فقط
            await _unitOfWork.GovernratesShippingCosts.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.GovernratesShippingCosts.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }

}
