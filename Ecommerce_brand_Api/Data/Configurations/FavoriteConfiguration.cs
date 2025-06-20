
namespace Ecommerce_brand_Api.Data.Configurations
{
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId);

            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            builder.HasIndex(f => new { f.UserId, f.ProductId }).IsUnique();

            builder.HasQueryFilter(f => !f.IsDeleted);
        }
    }
}
