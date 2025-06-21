
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce_brand_Api.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.TotalBasePrice).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Discount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.TotalAmount).HasColumnType("decimal(18,2)");


            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);
        }

    }
    }
