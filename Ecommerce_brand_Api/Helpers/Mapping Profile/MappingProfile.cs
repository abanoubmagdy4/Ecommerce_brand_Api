using Ecommerce_brand_Api.Models.Dtos;

namespace Ecommerce_brand_Api.Helpers.Mapping_Profile
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<GovernrateShippingCostDto, GovernorateShippingCost>();
            CreateMap<GovernorateShippingCost, GovernrateShippingCostDto>();


            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            // Sizes
            CreateMap<ProductSizes, ProductSizesDto>().ReverseMap();

            // Images
            CreateMap<ProductImagesPaths, ProductImagesPathsDto>()
                .ForMember(dest => dest.File, opt => opt.Ignore());

            CreateMap<ProductImagesPathsDto, ProductImagesPaths>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath ?? ""));



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




            CreateMap<Feedback, FavoriteDto>();
            CreateMap<FeedbackDto, Feedback>();

            CreateMap<OrderDTO, Order>();
            CreateMap<Order, OrderDTO>();

            CreateMap<OrderItemDTO, OrderItem>();
            CreateMap<OrderItem, OrderItemDTO>();





            CreateMap<Address, AddressDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<AddressDto, Address>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
                .ForMember(dest => dest.GovernorateShippingCostId, opt => opt.MapFrom(src => src.GovernrateShippingCostId))
                .ForMember(dest => dest.GovernorateShippingCost, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            CreateMap<GovernorateShippingCost, GovernrateShippingCostDto>().ReverseMap();
        }

    }

}

