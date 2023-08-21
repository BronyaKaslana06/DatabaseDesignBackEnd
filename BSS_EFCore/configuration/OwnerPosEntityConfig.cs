using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSS_EFCore.configuration
{
    internal class OwnerPosEntityConfig : IEntityTypeConfiguration<OwnerPos>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OwnerPos> builder)
        {
            builder.HasKey(o => o.OwnerId).HasName("SYS_C009099");

            builder.ToTable("OWNERPOS");

            builder.Property(o => o.OwnerId)
                .HasColumnName("Owner_ID");
            builder.Property(o => o.Address)
                .HasColumnName("ADDRESS");

            builder.HasOne<VehicleOwner>(o => o.vehicleowner).WithMany(a => a.ownerpos).IsRequired();
        }
    }
}
