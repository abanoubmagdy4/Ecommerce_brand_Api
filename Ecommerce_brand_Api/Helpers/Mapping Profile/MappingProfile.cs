using Ecommerce_brand_Api.Models.Dtos;
using Ecommerce_brand_Api.Models.Dtos.OrdersDTO;

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

                CreateMap<ProductDto, Product>();
                CreateMap<Product, ProductDto>();

                CreateMap<CartDto, Cart>();
                CreateMap<Cart, CartDto>();

                CreateMap<CartItemDto, CartItem>();
                CreateMap<CartItem, CartItemDto>();

                CreateMap<Feedback, FavoriteDto>();
                CreateMap<FeedbackDto, Feedback>();

                CreateMap<OrderDTO, Order>();
                CreateMap<Order, OrderDTO>();

                CreateMap<OrderItemDTO, OrderItem>();
                CreateMap<OrderItem, OrderItemDTO>();

                CreateMap<ProductImagesPaths, ProductImagesPathsDto>()
                    .ForMember(dest => dest.File, opt => opt.Ignore());

                CreateMap<ProductImagesPathsDto, ProductImagesPaths>()
                    .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath ?? ""));

                CreateMap<Address, AddressDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.GovernrateShippingCostDto, opt => opt.MapFrom(src => src.GovernorateShippingCost));

                CreateMap<AddressDto, Address>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0))
                    .ForMember(dest => dest.GovernorateShippingCostId, opt => opt.MapFrom(src => src.GovernrateShippingCostDto.Id))
                    .ForMember(dest => dest.GovernorateShippingCost, opt => opt.Ignore())
                    .ForMember(dest => dest.UserId, opt => opt.Ignore())
                    .ForMember(dest => dest.User, opt => opt.Ignore())
                    .ForMember(dest => dest.Order, opt => opt.Ignore());

                CreateMap<GovernorateShippingCost, GovernrateShippingCostDto>().ReverseMap();
            }
        }

    }

