using Ecommerce_brand_Api.Helpers;
using Ecommerce_brand_Api.Helpers.Mapping_Profile;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Ecommerce_brand_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost4200", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // Angular port
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddHttpClient();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddHttpContextAccessor();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            builder.Services.AddScoped<IUnitofwork, Unitofwork>();
            builder.Services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, CartServices>();
            builder.Services.AddScoped<IRefundRequestRepository, RefundRequestRepository>();
            builder.Services.AddScoped<IRefundRequestService, RefundRequestService>();
            builder.Services.AddScoped<IRefundRequestService, RefundRequestService>();
            builder.Services.AddScoped<IGovernrateShippingCostRepository, GovernrateShippingCostRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddScoped<IProductSizesRepository, ProductsSizesRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<INewArrivalsRepository, NewArrivalsRepository>();

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICartService, CartServices>();
            builder.Services.AddScoped<IGovernrateShippingCostService, GovernrateShippingCostService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<INewArrivalsService, NewArrivalsService>();

            builder.Services.AddHttpContextAccessor();


            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IGovernrateShippingCostService, GovernrateShippingCostService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddScoped<IOrderService, OrderServices>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

            builder.Services.AddSwaggerGen(c =>
            {

                c.EnableAnnotations();

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ecommerce Brand API",
                    Version = "1.0.0"
                });


                // دعم تحميل الصور
                c.SchemaFilter<FormFileSchemaFilter>();

                // ✅ إضافة دعم الـ JWT في Swagger

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
          {
              new OpenApiSecurityScheme
              {
                  Reference = new OpenApiReference
                  {
                      Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  }
              },
              new string[] {}
          }
      });
            });

            builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
            var app = builder.Build();
            app.UseStaticFiles();

            app.UseSwagger(options => options.OpenApiVersion =
            Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce Brand API V1");

                });

            app.UseHttpsRedirection();
            app.UseCors("AllowLocalhost4200");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
