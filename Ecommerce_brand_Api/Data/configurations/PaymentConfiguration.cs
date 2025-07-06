using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Ecommerce_brand_Api.Data.configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {

        public void Configure(EntityTypeBuilder<Payment> builder)
        {

            builder.Property(p => p.Currency)
               .HasMaxLength(10)
               .IsRequired();

            builder.Property(p => p.PaymentStatus)
                .HasMaxLength(20)
            .IsRequired();
            builder
               .HasIndex(p => p.TransactionId)
                .IsUnique();

            builder.Property(p => p.Status)
         .HasConversion<string>();
            //Relationships
            builder
            .HasMany(p => p.Items)
            .WithOne(i => i.Payment)
            .HasForeignKey(i => i.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

            builder
            .HasOne(p => p.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(p => p.OrderRefunds)
                .WithOne(r => r.Payment)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);




        }
    }
}
