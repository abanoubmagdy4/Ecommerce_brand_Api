
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Ecommerce_brand_Api.Data.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Rating)
                .IsRequired();

            builder.HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId);

            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

          
        }
    }
}
