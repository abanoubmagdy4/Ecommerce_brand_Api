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

            CreateMap<CartDto, Cart>()
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Discount, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CartItemDto, CartItem>().ReverseMap();

            CreateMap<CartDto, Cart>();

            CreateMap<Cart, CartDto>();
            CreateMap<Feedback, FavoriteDto>();
            CreateMap<FeedbackDto, Feedback>();

            // Product ↔ ProductDto (نفترض إنك عامل ProductDto)
            CreateMap<Product, ProductDto>().ReverseMap();

            // ProductImagesPaths ↔ ProductImagesPathsDto
            CreateMap<ProductImagesPaths, ProductImagesPathsDto>()
                .ForMember(dest => dest.File, opt => opt.Ignore()); // مابش الـ IFormFile

            CreateMap<ProductImagesPathsDto, ProductImagesPaths>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath ?? ""));

            CreateMap<CartItem, CartItemDto>().ReverseMap();
        }
    }
}
