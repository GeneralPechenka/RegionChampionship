using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 14. EventLog
    public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
    {
        public void Configure(EntityTypeBuilder<EventLog> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EventType).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(1000);
            builder.Property(e => e.OccurredAt).HasDefaultValueSql("NOW()");

            // Связь
            builder.HasOne(e => e.VendingMachine)
                .WithMany()
                .HasForeignKey(e => e.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы для анализа
            builder.HasIndex(e => new { e.VendingMachineId, e.OccurredAt });
            builder.HasIndex(e => e.EventType);
        }
    }
}
