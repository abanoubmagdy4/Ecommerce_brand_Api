using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce_brand_Api.Data.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
        }
    }
}
