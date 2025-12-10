using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 5. Company
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.Name).IsUnique();
            builder.HasIndex(c => c.Email).IsUnique();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.Phone).HasMaxLength(20);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("NOW()");
        }
    }
}
