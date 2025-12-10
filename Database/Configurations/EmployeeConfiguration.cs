using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 6. Employee
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            // Альтернативные ключи (уникальные поля)
            builder.HasAlternateKey(e => e.Email);

            builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Role).HasDefaultValue(EmployeeRoleEnum.Operator);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

            // Связи
            builder.HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
