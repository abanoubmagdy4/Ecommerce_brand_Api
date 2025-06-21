using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce_brand_Api.Data.Configurations
{
    public class GovernrateShippingCostConfiguration : IEntityTypeConfiguration<GovernorateShippingCost>
    {
        public void Configure(EntityTypeBuilder<GovernorateShippingCost> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("nvarchar(100)");

            builder.Property(c => c.ShippingCost)
                   .HasColumnType("decimal(18,2)");
            builder.HasIndex(c => c.Name).IsUnique();
        }

    }
}
