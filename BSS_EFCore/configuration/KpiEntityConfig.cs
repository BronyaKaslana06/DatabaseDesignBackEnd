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
    internal class KpiEntityConfig : IEntityTypeConfiguration<Kpi>
    {
        public void Configure(EntityTypeBuilder<Kpi> builder)
        {
            builder.HasKey(e => e.KpiId).HasName("SYS_C009105");

            builder.ToTable("KPI");
  
            builder.Property(e => e.KpiId)
                .HasColumnName("KPI_ID");
            builder.Property(e => e.Score)
                .HasColumnName("SCORE");
            builder.Property(e => e.ServiceFrequency)
                .HasColumnName("SERVICE_FREQUENCY");
            builder.Property(e => e.TotalPerformance)
                .HasColumnName("TOTAL_PERFORMANCE");

        }
    }
}
