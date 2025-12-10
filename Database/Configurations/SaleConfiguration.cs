using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 10. Sale
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Quantity).IsRequired();
            builder.Property(s => s.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(s => s.SaleDate).HasDefaultValueSql("NOW()");
            builder.Property(s => s.PaymentMethod).IsRequired();

            builder.ToTable(t => t.HasCheckConstraint("CK_Quantity", "\"Quantity\" > 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Amount", "\"Amount\" > 0"));

            // Связи
            builder.HasOne(s => s.VendingMachine)
                .WithMany(v => v.Sales)
                .HasForeignKey(s => s.VendingMachineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индексы для быстрого поиска
            builder.HasIndex(s => s.SaleDate);
            builder.HasIndex(s => new { s.VendingMachineId, s.SaleDate });
            builder.HasIndex(s => new { s.ProductId, s.SaleDate });
        }
    }
}
