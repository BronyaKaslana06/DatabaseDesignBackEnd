using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSS_EFCore.configuration
{
    internal class SwitchRequestEntityConfig : IEntityTypeConfiguration<SwitchRequest>
    {
        public void Configure(EntityTypeBuilder<SwitchRequest> builder)
        {
            builder.HasKey(e => e.SwitchRequestId).HasName("SYS_C008772");

            builder.ToTable("SWITCH_REQUEST");

            builder.Property(e => e.SwitchRequestId)
                .HasColumnName("SWITCH_REQUEST_ID");
            builder.Property(e => e.Position)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("POSITION");
            builder.Property(e => e.Note)
                .HasMaxLength(255)
                .IsUnicode()
                .HasColumnName("NOTES");
            builder.Property(e => e.RequestTime)
                .HasColumnName("REQUEST_TIME");
            builder.Property(e => e.SwitchType)
                .HasColumnName("SWITCH_TYPE");

            builder.HasOne<VehicleOwner>(e => e.vehicleOwner).WithMany(a => a.switchRequests).IsRequired();
            builder.HasOne<Vehicle>(e => e.vehicle).WithMany(a=>a.switchRequests).IsRequired();

            builder.HasOne<BatteryType>(e=>e.batteryType).WithMany(a=>a.switchRequests).IsRequired();
        }
    }
}
