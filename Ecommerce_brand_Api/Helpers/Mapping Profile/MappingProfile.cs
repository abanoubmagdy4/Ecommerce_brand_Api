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

            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Threshold, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDiscount, opt => opt.Ignore())
                .ForSourceMember(dest => dest.User, opt => opt.DoNotValidate())
                .ReverseMap();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ReverseMap();


            CreateMap<Feedback, FavoriteDto>();
            CreateMap<FeedbackDto, Feedback>();

            // Product ↔ ProductDto (نفترض إنك عامل ProductDto)
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<ProductSizes, ProductSizeDto>().ReverseMap();

            // ProductImagesPaths ↔ ProductImagesPathsDto
            CreateMap<ProductImagesPaths, ProductImagesPathsDto>()
                .ForMember(dest => dest.File, opt => opt.Ignore()); // مابش الـ IFormFile

            CreateMap<ProductImagesPathsDto, ProductImagesPaths>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath ?? ""));

        }
    }
}
