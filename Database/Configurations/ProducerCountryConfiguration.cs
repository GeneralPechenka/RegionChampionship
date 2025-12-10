using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 4. ProducerCountry
    public class ProducerCountryConfiguration : IEntityTypeConfiguration<ProducerCountry>
    {
        public void Configure(EntityTypeBuilder<ProducerCountry> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Code).IsUnique();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Code).IsRequired().HasMaxLength(10);
        }
    }
}
