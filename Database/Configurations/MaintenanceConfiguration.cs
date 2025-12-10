using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 11. Maintenance
    public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.MaintenanceDate).HasDefaultValueSql("NOW()");
            builder.Property(m => m.WorkDescription).HasMaxLength(2000);
            builder.Property(m => m.Problems).HasMaxLength(2000);
            builder.Property(m => m.Type).IsRequired();

            // Связи
            builder.HasOne(m => m.VendingMachine)
                .WithMany(v => v.Maintenances)
                .HasForeignKey(m => m.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Employee)
                .WithMany(e => e.Maintenances)
                .HasForeignKey(m => m.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
