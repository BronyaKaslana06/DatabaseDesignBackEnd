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
    internal class SwitchLogEntityConfig : IEntityTypeConfiguration<SwitchLog>
    {
        public void Configure(EntityTypeBuilder<SwitchLog> builder)
        {
            builder.HasKey(e => e.SwitchServiceId).HasName("SYS_C009138");

            builder.ToTable("SWITCH_LOG");

            builder.Property(e => e.SwitchServiceId)
                .HasColumnName("SWITCH_SERVICE_ID");
            builder.Property(e => e.SwitchTime)
                .HasColumnName("SWITCH_TIME");
            builder.Property(e => e.Score)
                .HasColumnName("SCORE");


            builder.HasOne<Battery>(d => d.batteryOn).WithMany(p => p.switchLogsOn).IsRequired();

            builder.HasOne<Battery>(d => d.batteryOff).WithMany(p => p.switchLogsOff).IsRequired();

            builder.HasOne(d => d.employee).WithMany(p => p.switchLogs).IsRequired();

            builder.HasOne<Vehicle>(d => d.vehicle).WithMany(p=>p.SwitchLogs).IsRequired();


        }
    }
}
