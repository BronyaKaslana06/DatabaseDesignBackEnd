using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSS_EFCore.configuration
{
    internal class BatteryEntityConfig : IEntityTypeConfiguration<Battery>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Battery> builder)
        {
            builder.HasKey(e => e.BatteryId).HasName("SYS_C009076");

            builder.ToTable("BATTERY");

            builder.Property(e => e.BatteryId)
                .HasColumnName("BATTERY_ID");
            builder.Property(e => e.AvailableStatus)
                .HasColumnName("AVAILABLE_STATUS");
            builder.Property(e => e.CurrChargeTimes)
                .HasColumnName("CURR_CHARGE_TIMES");
            builder.Property(e => e.CurrentCapacity)
                .HasColumnName("CURRENT_CAPACITY");
            builder.Property(e => e.ManufacturingDate)
                .HasColumnName("MANUFACTURING_DATE");


            builder.HasOne<SwitchStation>(e => e.switchStation).WithMany(a => a.batteries);
            builder.HasOne<BatteryType>(e => e.batteryType).WithMany(a => a.batteries).IsRequired();
        }
    }
}
