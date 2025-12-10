using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 13. Notification
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Message).IsRequired().HasMaxLength(500);
            builder.Property(n => n.Type).IsRequired();
            builder.Property(n => n.CreatedAt).HasDefaultValueSql("NOW()");
            builder.Property(n => n.IsRead).HasDefaultValue(false);

            // Связи
            builder.HasOne(n => n.VendingMachine)
                .WithMany(v => v.Notifications)
                .HasForeignKey(n => n.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы
            builder.HasIndex(n => new { n.UserId, n.IsRead });
            builder.HasIndex(n => n.CreatedAt);
        }
    }
}
