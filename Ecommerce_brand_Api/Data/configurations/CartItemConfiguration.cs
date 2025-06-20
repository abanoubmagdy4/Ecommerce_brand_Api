
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

            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
        }


    }
}
