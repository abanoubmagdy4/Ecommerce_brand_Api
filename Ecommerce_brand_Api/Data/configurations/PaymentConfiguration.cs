using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce_brand_Api.Data.configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
           
        }
    }
}
