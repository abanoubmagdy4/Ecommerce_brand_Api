﻿using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Services.Interfaces
{
    public interface IGovernrateShippingCostService : IBaseService<GovernorateShippingCost>
    {
        Task<IEnumerable<GovernrateShippingCostDto>> GetAllAsync();
        Task<GovernrateShippingCostDto?> GetByIdAsync(int id);
        Task<GovernrateShippingCostDto?> GetByNameAsync(string name);
        Task AddAsync(GovernrateShippingCostDto dto);
        Task<bool> UpdateAsync(int id, GovernrateShippingCostDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
