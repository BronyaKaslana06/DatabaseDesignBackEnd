using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSS_EFCore.configuration
{
    internal class NewsEntityConfig : IEntityTypeConfiguration<News>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<News> builder)
        {
            builder.HasKey(e => e.AnnouncementId).HasName("SYS_C009098");

            builder.ToTable("NEWS");

            builder.Property(e => e.AnnouncementId)
                .HasColumnName("ANNOUNCEMENT_ID");
            builder.Property(e => e.Contents)
                .HasMaxLength(2500)
                .IsUnicode(true)
                .HasColumnName("CONTENTS");
            builder.Property(e => e.Likes)
                .HasColumnName("LIKES");
            builder.Property(e => e.PublishPos)
                .HasMaxLength(50)
                .IsUnicode(true)
                .HasColumnName("PUBLISH_POS");
            builder.Property(e => e.PublishTime)
                .HasColumnName("PUBLISH_TIME");
            builder.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(true)
                .HasColumnName("TITLE");
            builder.Property(e => e.ViewCount)
                .HasColumnName("VIEW_COUNT");

            builder.HasOne<Administrator>(e => e.administrator).WithMany(a => a.news).IsRequired();
        }
    }
}
