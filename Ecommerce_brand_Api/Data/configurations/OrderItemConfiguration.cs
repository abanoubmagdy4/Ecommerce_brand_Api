namespace Ecommerce_brand_Api.Data.configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            //============================= Fluent API ==========================================


            //============================= OrderItem Entitiy ==========================================

            builder.HasKey(oi => oi.OrderItemId);

            // Relationships
            builder
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Properties
            builder
                .Property(oi => oi.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder
                .Property(oi => oi.Quantity)
                .IsRequired();

            builder
                .HasIndex(oi => new { oi.OrderId, oi.ProductId })
                .IsUnique();
            //-------------------------------------------------------------------------------------------------------
        }
    }
}
