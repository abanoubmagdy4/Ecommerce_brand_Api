
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce_brand_Api.Data.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(ci => ci.TotalPriceForOneItemType).HasColumnType("decimal(18,2)");

            // ✅ العلاقة مع Cart (Cascade مسموحة)
            builder.HasOne(c => c.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(c => c.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ العلاقة مع ProductSize (لازم Restrict)
            builder.HasOne(c => c.ProductSize)
                .WithMany()
                .HasForeignKey(c => c.ProductSizeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ العلاقة مع Product (لازم Restrict)
            builder.HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }



    }
}
