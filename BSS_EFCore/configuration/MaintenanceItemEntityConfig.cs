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
    internal class MaintenanceItemEntityConfig : IEntityTypeConfiguration<MaintenanceItem>
    {
        public void Configure(EntityTypeBuilder<MaintenanceItem> builder)
        {
            builder.HasKey(e => e.MaintenanceItemId).HasName("SYS_C009117");

            builder.ToTable("MAINTENANCE_ITEM");

            //builder.HasIndex(e => e.MaintenanceLocation, "SYS_C009118").IsUnique();

            builder.Property(e => e.MaintenanceItemId)
                .HasColumnName("MAINTENANCE_ITEM_ID");
            builder.Property(e => e.MaintenanceLocation)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("MAINTENANCE_LOCATION");
            builder.Property(e => e.OrderStatus)
                .HasColumnName("ORDER_STATUS");
            builder.Property(e => e.OrderSubmissionTime)
                .HasPrecision(6)
                .HasColumnName("ORDER_SUBMISSION_TIME");
            builder.Property(e => e.ServiceTime)
                .HasColumnName("SERVICE_TIME");
            builder.Property(e => e.AppointTime)
                .HasColumnName("APPOINT_TIME");
            builder.Property(e => e.Note)
                .HasColumnName("NOTE");
            builder.Property(e => e.Score)
                .HasColumnName("SCORE");
            builder.Property(e => e.Title)
                .HasColumnName("TITLE");

            builder.HasMany<Employee>(e => e.employees).WithMany(a => a.maintenanceItems).UsingEntity(j=>j.ToTable("Employee_MaintenanceItem"));
            builder.HasOne<Vehicle>(e=>e.vehicle).WithMany(a=>a.maintenanceItems).IsRequired();
        }
    }
}
