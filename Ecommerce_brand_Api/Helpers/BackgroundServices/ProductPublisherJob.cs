using AutoMapper;
using Ecommerce_brand_Api.Helpers.Hubs;
using Ecommerce_brand_Api.Models.Dtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_brand_Api.Helpers.BackgroundServices
{
    public class ProductPublisherJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;


        public ProductPublisherJob(
            IProductRepository productRepository,
            IHubContext<ProductHub> hubContext,
            AppDbContext context,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
        }

        public async Task PublishProduct(int productId)
        {
            var product = await _productRepository.GetByIdWithImagesAsync(productId);
            if (product == null || product.IsPublished)
                return;

            product.IsPublished = true;
            product.PublishAt = DateTime.UtcNow;

            _context.Products.Attach(product);
            await _context.SaveChangesAsync();
            var productResponsedto = _mapper.Map<ProductDtoResponse>(product);

            if (productResponsedto.PublishAt.HasValue)
            {
                var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                productResponsedto.PublishAt = TimeZoneInfo.ConvertTimeFromUtc(productResponsedto.PublishAt.Value, egyptTimeZone);
            }

            await _hubContext.Clients.All.SendAsync("NewProductsArrived", new List<ProductDtoResponse> { productResponsedto });

        }
    }
    }
