using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations
{
    // 3. VendingAndPayType
    public class VendingAndPayTypeConfiguration : IEntityTypeConfiguration<VendingAndPayType>
    {
        public void Configure(EntityTypeBuilder<VendingAndPayType> builder)
        {
            builder.HasKey(vp => new { vp.VendingMachineId, vp.PayTypeId });

            builder.HasOne(vp => vp.VendingMachine)
                .WithMany(v => v.PaymentTypes)
                .HasForeignKey(vp => vp.VendingMachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vp => vp.PayType)
                .WithMany(p => p.VendingMachines)
                .HasForeignKey(vp => vp.PayTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
