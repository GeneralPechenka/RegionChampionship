using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Database.Configurations
{
    // 1. VendingMachine
    public class VendingMachineConfiguration : IEntityTypeConfiguration<VendingMachine>
    {
        public void Configure(EntityTypeBuilder<VendingMachine> builder)
        {
            builder.HasKey(v => v.Id);

            // Уникальные индексы
            builder.HasIndex(v => v.SerialNumber).IsUnique();
            builder.HasIndex(v => v.InventoryNumber).IsUnique();

            // Ограничения полей
            builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
            builder.Property(v => v.Location).HasMaxLength(500);
            builder.Property(v => v.Address).HasMaxLength(500);
            builder.Property(v => v.Model).HasMaxLength(100);
            builder.Property(v => v.TotalRevenue).HasPrecision(18, 2);
            builder.Property(v => v.SerialNumber).IsRequired().HasMaxLength(50);
            builder.Property(v => v.InventoryNumber).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Manufacturer).HasMaxLength(200);
            builder.Property(v => v.ResourceHours).HasDefaultValue(0);
            builder.Property(v => v.MaintenanceDurationHours).HasDefaultValue(1);
            builder.Property(v => v.Status).HasDefaultValue(MachineStatusEnum.Working);
            builder.Property(v => v.CreatedAt).HasDefaultValueSql("NOW()");

            // Ограничения полей
            builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
            builder.Property(v => v.Location).HasMaxLength(500);
            builder.Property(v => v.Address).HasMaxLength(500);
            builder.Property(v => v.Model).HasMaxLength(100);
            builder.Property(v => v.TotalRevenue).HasPrecision(18, 2);
            builder.Property(v => v.SerialNumber).IsRequired().HasMaxLength(50);
            builder.Property(v => v.InventoryNumber).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Manufacturer).HasMaxLength(200);
            builder.Property(v => v.ResourceHours).HasDefaultValue(0);
            builder.Property(v => v.MaintenanceDurationHours).HasDefaultValue(1);
            builder.Property(v => v.Status).HasDefaultValue(MachineStatusEnum.Working);
            builder.Property(v => v.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Проверочные ограничения для СУБД (применяются к builder, а не property)
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_VendingMachine_CommissioningDate",
                "\"CommissioningDate\" >= \"ManufactureDate\""
            ));

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_VendingMachine_LastVerificationDate",
                "(\"LastVerificationDate\" IS NULL) OR " +
                "(\"LastVerificationDate\" >= \"ManufactureDate\" AND " +
                "\"LastVerificationDate\" <= CURRENT_TIMESTAMP)"
            ));

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_VendingMachine_InventoryDate",
                "(\"InventoryDate\" IS NULL) OR " +
                "(\"InventoryDate\" >= \"ManufactureDate\" AND " +
                "\"InventoryDate\" <= CURRENT_TIMESTAMP)"
            ));

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_VendingMachine_NextMaintenanceDate",
                "(\"NextMaintenanceDate\" IS NULL) OR " +
                "(\"NextMaintenanceDate\" > \"CreatedAt\")"
            ));

            // Проверки из задания:
            // 1. Дата ввода в эксплуатацию не может быть раньше даты изготовления
            // 2. Дата последней поверки не может быть раньше даты изготовления и позже текущей даты
            // 3. Дата инвентаризации не может быть раньше даты изготовления и позже текущей даты
            // 4. Дата следующего ремонта должна быть позже даты внесения ТА в систему

            // Связи
            builder.HasOne(v => v.Company)
                .WithMany(c => c.VendingMachines)
                .HasForeignKey(v => v.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(v => v.Modem)
                .WithOne(m => m.VendingMachine)
                .HasForeignKey<Modem>(m => m.Id)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(v => v.ProducerCountry)
                .WithMany(p => p.VendingMachines)
                .HasForeignKey(v => v.ProducerCountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.LastVerificationEmployee)
                .WithMany(e => e.VerifiedMachines)
                .HasForeignKey(v => v.LastVerificationEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
