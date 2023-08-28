using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSS_EFCore.configuration
{
    internal class EmployeeEntityConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.EmployeeId).HasName("SYS_C009095");

            builder.ToTable("EMPLOYEE");

            builder.Property(e => e.EmployeeId)
                .HasColumnName("EMPLOYEE_ID");
            builder.Property(e => e.AccountSerial)
                .ValueGeneratedNever()
                .HasColumnName("ACCOUNT_SERIAL");
            builder.Property(e => e.CreateTime)
                .HasColumnName("CREATE_TIME");
            builder.Property(e => e.Gender)
                .HasMaxLength(3)
                .IsUnicode()
                .HasColumnName("GENDER");
            builder.Property(e => e.IdentityNumber)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("IDENTITYNUMBER");
            builder.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode()
                .HasColumnName("NAME");
            builder.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode()
                .HasColumnName("USERNAME");
            builder.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode()
                .HasColumnName("PASSWORD")
                .HasDefaultValue("123456");
            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode()
                .HasColumnName("PHONE_NUMBER");
            builder.Property(e => e.Position)
                .HasColumnName("POSITIONS");
            builder.Property(e => e.ProfilePhoto)
                .HasColumnType("BLOB")
                .HasColumnName("PROFILE_PHOTO");
            builder.Property(e => e.Salary)
                .IsRequired()
                .HasColumnName("SALARY");

            builder.HasOne<Kpi>(e => e.kpi).WithOne(a => a.employee).HasForeignKey<Kpi>(c=>c.employeeId).IsRequired();
            builder.HasOne<SwitchStation>(e => e.switchStation).WithMany(a => a.employees);
        }
    }
}
