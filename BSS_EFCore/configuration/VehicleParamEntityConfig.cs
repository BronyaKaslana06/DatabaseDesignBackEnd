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
    internal class VehicleParamEntityConfig : IEntityTypeConfiguration<VehicleParam>
    {
        public void Configure(EntityTypeBuilder<VehicleParam> builder)
        {
            builder.HasKey(e => e.VehicleModelId).HasName("SYS_C009081");

            builder.ToTable("VEHICLE_PARAM");

            builder.Property(e => e.VehicleModelId)
                .HasColumnName("VEHICLE_MODEL");
            builder.Property(e => e.Manufacturer)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("MANUFACTURER");
            builder.Property(e => e.MaxSpeed)
                .HasColumnName("MAX_SPEED");
            builder.Property(e => e.ServiceTerm)
                .HasColumnName("SERVICE_TERM");
            builder.Property(e => e.Transmission)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("TRANSMISSION");
            builder.Property(e => e.Sinp)
                .HasColumnType("BLOB")
                .HasColumnName("SINP");

        }
    }
}
