using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Helpers.Mapping_Profile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map from DTO to Entity
            CreateMap<CategoryDto, Category>();

            CreateMap<Category, CategoryDto>();

            CreateMap<GovernrateShippingCostDto, GovernorateShippingCost>();

            CreateMap<GovernorateShippingCost, GovernrateShippingCostDto>();
        }
    }
}
