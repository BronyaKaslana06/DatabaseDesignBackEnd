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
    internal class SwitchStationEntityConfig : IEntityTypeConfiguration<SwitchStation>
    {
        public void Configure(EntityTypeBuilder<SwitchStation> builder)
        {
            builder.HasKey(e => e.StationId).HasName("SYS_C009065");

            builder.ToTable("SWITCH_STATION");

            //builder.HasIndex(e => e.Longtitude, "SYS_C009066").IsUnique();

            //builder.HasIndex(e => e.Latitude, "SYS_C009067").IsUnique();

            builder.Property(e => e.StationId)
                .HasColumnName("STATION_ID");
            builder.Property(e => e.AvailableBatteryCount)
                .HasColumnName("AVAILABLE_BATTERY_COUNT");
            builder.Property(e => e.BatteryCapacity)
                .HasColumnName("BATTERY_CAPACITY");
            builder.Property(e => e.FailureStatus)
                .HasColumnName("FALIURE_STATUS");
            builder.Property(e => e.Latitude)
                .HasColumnName("LATITUDE");
            builder.Property(e => e.Longtitude)
                .HasColumnName("LONGTITUDE");
            builder.Property(e => e.ServiceFee)
                .HasColumnName("SERVICE_FEE");
            builder.Property(e => e.StationName)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("STATION_NAME");

        }
    }
}
