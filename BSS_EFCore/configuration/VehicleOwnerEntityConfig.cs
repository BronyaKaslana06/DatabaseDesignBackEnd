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
    internal class VehicleOwnerEntityConfig : IEntityTypeConfiguration<VehicleOwner>
    {
        public void Configure(EntityTypeBuilder<VehicleOwner> builder)
        {
            builder.HasKey(e => e.OwnerId).HasName("SYS_C009088");

            builder.ToTable("VEHICLE_OWNER");

            builder.Property(e => e.OwnerId)
                .HasColumnName("OWNER_ID");
            builder.Property(e => e.Birthday)
                .HasColumnName("BIRTHDAY");
            builder.Property(e => e.CreateTime)
                .HasColumnName("CREATE_TIME");
            builder.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("EMAIL");
            builder.Property(e => e.Gender)
                .HasMaxLength(3)
                .IsUnicode()
                .HasColumnName("GENDER");
            builder.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("USERNAME");
            builder.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("PASSWORD");
            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode()
                .HasColumnName("PHONE_NUMBER");
            builder.Property(e => e.ProfilePhoto)
                .HasColumnType("BLOB")
                .HasColumnName("PROFILE_PHOTO");
        }
    }
}
