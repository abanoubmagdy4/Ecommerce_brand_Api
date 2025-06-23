using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;

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

            CreateMap<ProductDto, Product>();

            CreateMap<Product, ProductDto>();

            CreateMap<CartDto, Cart>();

            CreateMap<Cart, CartDto>();
            CreateMap<Feedback, FavoriteDto>();
            CreateMap<FeedbackDto, Feedback>();

            CreateMap<OrderDTO, Order>();

            CreateMap<Order, OrderDTO>();

            CreateMap<OrderItemDTO, OrderItem>();
            CreateMap<OrderItem, OrderItemDTO>();
            // Product ↔ ProductDto (نفترض إنك عامل ProductDto)
            CreateMap<Product, ProductDto>().ReverseMap();

            // ProductImagesPaths ↔ ProductImagesPathsDto
            CreateMap<ProductImagesPaths, ProductImagesPathsDto>()
                .ForMember(dest => dest.File, opt => opt.Ignore()); // مابش الـ IFormFile

            CreateMap<ProductImagesPathsDto, ProductImagesPaths>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath ?? ""));

        }
    }
}
