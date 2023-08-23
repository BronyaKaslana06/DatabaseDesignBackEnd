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
    internal class VehicleEntityConfig : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(e => e.VehicleId).HasName("SYS_C009110");

            builder.ToTable("VEHICLE");

            builder.Property(e => e.VehicleId)
                .HasColumnName("VEHICLE_ID");
            builder.Property(e => e.PurchaseDate)
                .HasColumnName("PURCHASE_DATE");
            builder.Property(e => e.PlateNumber)
                .HasColumnName("PLATE_NUMBER");


            builder.HasOne<VehicleOwner>(e => e.vehicleOwner).WithMany(a => a.vehicles).IsRequired();
            builder.HasOne<Battery>(e => e.Battery).WithOne(a => a.vehicle).IsRequired().HasForeignKey<Vehicle>(c => c.BatteryId);
            builder.HasOne<VehicleParam>(e=>e.vehicleParam).WithMany(a => a.vehicles).IsRequired();

        }
    }
}
