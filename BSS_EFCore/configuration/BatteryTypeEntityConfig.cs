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
    internal class BatteryTypeEntityConfig : IEntityTypeConfiguration<BatteryType>
    {
        public void Configure(EntityTypeBuilder<BatteryType> builder)
        {
            builder.HasKey(e => e.BatteryTypeId).HasName("SYS_C009070");

            builder.ToTable("BATTERY_TYPE");

            builder.Property(e => e.BatteryTypeId)
                .HasColumnName("BATTERY_TYPE_ID");
            builder.Property(e => e.MaxChargeTimes)
                .HasColumnName("MAX_CHARGE_TIEMS");
            builder.Property(e => e.TotalCapacity)
                .HasColumnName("TOTAL_CAPACITY");
        }
    }
}
