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
    internal class AdministratorEntityConfig : IEntityTypeConfiguration<Administrator>
    {
        public void Configure(EntityTypeBuilder<Administrator> builder)
        {
            builder.HasKey(e => e.AdminId).HasName("SYS_C009148");
            
            builder.ToTable("ADMINISTRATOR");

            builder.Property(e => e.AdminId)
                .HasColumnName("ADMIN_ID");
            builder.Property(e => e.AccountSerial)
                .ValueGeneratedNever()
                .HasColumnName("ACCOUNT_SERIAL");
            builder.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("PASSWORD");
        }
    }
}
