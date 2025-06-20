
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce_brand_Api.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(250);

            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
