using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 8. Product
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Name).IsUnique();

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.MinStock).HasDefaultValue(5);
            builder.Property(p => p.SalesTendency).HasPrecision(10, 2).HasDefaultValue(0);
            builder.Property(p => p.Category).HasMaxLength(100);

            builder.ToTable(t => t.HasCheckConstraint("CK_Product_Price", "\"Price\" > 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Product_MinStock", "\"MinStock\" >= 0"));

        }
    }
}
