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

            // Sizes
            CreateMap<ProductSizes, ProductSizesDto>(); // من الكيان للـ DTO (عرض البيانات)

            CreateMap<ProductSizesDto, ProductSizes>()  // من DTO للكيان (إضافة أو تعديل)
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // تجاهل الـ Id
            // Images
            // From Entity to DTO (Response)
            CreateMap<ProductImagesPaths, ProductImagesPathsDto>();

            // From DTO (Request) to Entity
            CreateMap<ProductImagesPathsDto, ProductImagesPaths>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath ?? ""));

            CreateMap<ProductDtoRequest, Product>()
             .ForMember(dest => dest.ProductImagesPaths, opt => opt.Ignore()) // هندخل الصور يدويًا
             .ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(src =>
                 src.Price - (src.Price * src.DiscountPercentage / 100)))
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now));

            //CreateMap<Product, ProductDtoRequest>();

            CreateMap<Product, ProductDtoResponse>();

            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Threshold, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDiscount, opt => opt.Ignore())
                .ForSourceMember(dest => dest.User, opt => opt.DoNotValidate())
                .ReverseMap();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl,
                    opt => opt.MapFrom(src => src.Product.ProductImagesPaths
                        .Where(img => img.Priority == 1)
                        .Select(img => img.ImagePath)
                        .FirstOrDefault()))

                .ForMember(dest => dest.ProductSizeName, opt => opt.MapFrom(src => src.ProductSize.Size))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.TotalPriceForOneItemType, opt => opt.MapFrom(src => src.TotalPriceForOneItemType)); // ← ممكن تضيف دي للتأكيد



            CreateMap<CartItemDto, CartItem>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Cart, opt => opt.Ignore())
            .ForMember(dest => dest.ProductSize, opt => opt.Ignore());



            CreateMap<Feedback, FavoriteDto>();
            CreateMap<FeedbackDto, Feedback>();

            CreateMap<Product, ProductDtoRequest>();


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

