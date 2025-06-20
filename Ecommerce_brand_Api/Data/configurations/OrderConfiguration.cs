namespace Ecommerce_brand_Api.Data.configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            //============================= Fluent API ==========================================

            //============================= Order Entitiy ==========================================
            builder.HasKey(o => o.OrderId);

            //<<<<<<<<<<<<<<<<<<<<<<< Relations >>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //Order-> ShippingAddress 
            builder
                .HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            //Order-> Customer 
            builder
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Order -> Discount 
            builder
                 .HasOne(o => o.Discount)
                 .WithMany()
                 .HasForeignKey(o => o.DiscountId)
                 .OnDelete(DeleteBehavior.SetNull);

            // Order -> Payment 
            builder
                .HasOne(o => o.Payment)
                .WithOne(c => c.Order)
                .HasForeignKey<Payment>(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid multiple paths

            // Order -> Cancellation 
            builder
                .HasOne(o => o.Cancelation)
                .WithOne(c => c.Order)
                .HasForeignKey<Cancelation>(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to avoid multiple paths

            //<<<<<<<<<<<<<<<<<<<<<<< Property >>>>>>>>>>>>>>>>>>>>>>>>>>>>
            builder
                .HasIndex(o => o.OrderNumber) 
                .IsUnique()
                .IsClustered(false);

            builder
                .Property(o => o.OrderNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)");

            builder
                .Property(o => o.OrderStatus)
                .HasConversion<string>();

        }
    }
}
