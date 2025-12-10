using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 12. EngineerTask
    public class EngineerTaskConfiguration : IEntityTypeConfiguration<EngineerTask>
    {
        public void Configure(EntityTypeBuilder<EngineerTask> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Description).HasMaxLength(2000);
            builder.Property(t => t.DueDate).IsRequired();
            builder.Property(t => t.Status).HasDefaultValue(TaskStatusEnum.New);
            builder.Property(t => t.Priority).HasDefaultValue(0);
            builder.Property(t => t.EstimatedHours).HasDefaultValue(1);

            builder.ToTable(t => t.HasCheckConstraint("CK_EstimatedHours", "\"EstimatedHours\" >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Priority", "\"Priority\" BETWEEN 0 AND 5"));


            // Связи
            builder.HasOne(t => t.VendingMachine)
                .WithMany()
                .HasForeignKey(t => t.VendingMachineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // Индексы
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.DueDate);
            builder.HasIndex(t => new { t.AssignedToId, t.Status });
        }
    }
}
