using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 9. ProductStock
    public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
    {
        public void Configure(EntityTypeBuilder<ProductStock> builder)
        {
            builder.HasKey(ps => ps.Id);

            // Альтернативный ключ для уникальности пары
            builder.HasAlternateKey(ps => new { ps.VendingMachineId, ps.ProductId });

            builder.Property(ps => ps.Quantity).HasDefaultValue(0);
            builder.Property(ps => ps.LastRestock).HasDefaultValueSql("NOW()");

            builder.ToTable(t => t.HasCheckConstraint("CK_Quantity", "\"Quantity\" >= 0"));


            // Связи
            builder.HasOne(ps => ps.VendingMachine)
                .WithMany(v => v.ProductStocks)
                .HasForeignKey(ps => ps.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Product)
                .WithMany(p => p.Stocks)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
