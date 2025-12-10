using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 2. PayType
    public class PayTypeConfiguration : IEntityTypeConfiguration<PayType>
    {
        public void Configure(EntityTypeBuilder<PayType> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Code).IsUnique();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(20);
        }
    }
}
