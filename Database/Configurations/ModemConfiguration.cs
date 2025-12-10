using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 7. Modem
    public class ModemConfiguration : IEntityTypeConfiguration<Modem>
    {
        public void Configure(EntityTypeBuilder<Modem> builder)
        {
            builder.HasKey(m => m.Id);
            builder.HasIndex(m => m.Imei).IsUnique();
            builder.HasIndex(m => m.PhoneNumber).IsUnique();

            builder.Property(m => m.Imei).IsRequired().HasMaxLength(15);
            builder.Property(m => m.Provider).HasMaxLength(50);
            builder.Property(m => m.PhoneNumber).HasMaxLength(20);
            builder.Property(m => m.ActivatedAt).HasDefaultValueSql("NOW()");
            builder.Property(m => m.IsActive).HasDefaultValue(true);
        }
    }
}
