using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
                 .WithMany(a => a.Order)
                .HasForeignKey(o => o.ShippingAddressId)
               .OnDelete(DeleteBehavior.Restrict);

            //Order-> Customer 
            builder
                 .HasOne(o => o.Customer)
                  .WithMany(o => o.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete


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

            builder.Property(o => o.paymentMethod)
                .HasConversion<string>();

        }
    }
}
