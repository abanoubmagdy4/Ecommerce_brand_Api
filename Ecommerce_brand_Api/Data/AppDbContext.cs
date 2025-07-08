using Ecommerce_brand_Api.Data.configurations;

namespace Ecommerce_brand_Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentItem> PaymentItems { get; set; }
        public DbSet<Cancelation> Cancellations { get; set; }
        public DbSet<OrderRefund> OrderRefund { get; set; }
        public DbSet<ProductRefund> ProductRefund { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<GovernorateShippingCost> GovernratesShippingCosts { get; set; }
        public DbSet<ProductSizes> ProductSizes { get; set; }
        public DbSet<NewArrivals> NewArrivals { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
            AppDbSeed.Seed(modelBuilder); // ✅ دي اللي ضفناها
            base.OnModelCreating(modelBuilder);
        }


    }
}
