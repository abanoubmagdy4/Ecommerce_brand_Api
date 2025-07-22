
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Ecommerce_brand_Api.Data.Configurations
{
   
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.AverageRating)
               .HasDefaultValue(0)
               .HasPrecision(3, 2);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
