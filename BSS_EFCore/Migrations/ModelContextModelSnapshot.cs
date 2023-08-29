﻿// <auto-generated />
using System;
using EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace BSS_EFCore.Migrations
{
    [DbContext(typeof(ModelContext))]
    partial class ModelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("C##CAR")
                .UseCollation("USING_NLS_COMP")
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EmployeeMaintenanceItem", b =>
                {
                    b.Property<long>("employeesEmployeeId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("maintenanceItemsMaintenanceItemId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("employeesEmployeeId", "maintenanceItemsMaintenanceItemId");

                    b.HasIndex("maintenanceItemsMaintenanceItemId");

                    b.ToTable("Employee_MaintenanceItem", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.Administrator", b =>
                {
                    b.Property<long>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("ADMIN_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AdminId"));

                    b.Property<string>("AccountSerial")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("ACCOUNT_SERIAL");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("PASSWORD");

                    b.HasKey("AdminId")
                        .HasName("SYS_C009148");

                    b.ToTable("ADMINISTRATOR", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.Battery", b =>
                {
                    b.Property<long>("BatteryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("BATTERY_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("BatteryId"));

                    b.Property<int?>("AvailableStatus")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("AVAILABLE_STATUS");

                    b.Property<long>("BatteryTypeId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<int>("CurrChargeTimes")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("CURR_CHARGE_TIMES");

                    b.Property<double>("CurrentCapacity")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("CURRENT_CAPACITY");

                    b.Property<DateTime>("ManufacturingDate")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("MANUFACTURING_DATE");

                    b.Property<long?>("switchStationStationId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("BatteryId")
                        .HasName("SYS_C009076");

                    b.HasIndex("BatteryTypeId");

                    b.HasIndex("switchStationStationId");

                    b.ToTable("BATTERY", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.BatteryType", b =>
                {
                    b.Property<long>("BatteryTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("BATTERY_TYPE_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("BatteryTypeId"));

                    b.Property<int>("MaxChargeTimes")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("MAX_CHARGE_TIEMS");

                    b.Property<string>("Name")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("TotalCapacity")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("TOTAL_CAPACITY");

                    b.HasKey("BatteryTypeId")
                        .HasName("SYS_C009070");

                    b.ToTable("BATTERY_TYPE", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.Employee", b =>
                {
                    b.Property<long>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("EMPLOYEE_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("EmployeeId"));

                    b.Property<string>("AccountSerial")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("ACCOUNT_SERIAL");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("CREATE_TIME");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Gender")
                        .HasMaxLength(3)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(3)")
                        .HasColumnName("GENDER");

                    b.Property<string>("IdentityNumber")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("IDENTITYNUMBER");

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(30)")
                        .HasColumnName("NAME");

                    b.Property<string>("Password")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasDefaultValue("123456")
                        .HasColumnName("PASSWORD");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(20)")
                        .HasColumnName("PHONE_NUMBER");

                    b.Property<int>("Position")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("POSITIONS");

                    b.Property<byte[]>("ProfilePhoto")
                        .HasColumnType("BLOB")
                        .HasColumnName("PROFILE_PHOTO");

                    b.Property<int>("Salary")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("SALARY");

                    b.Property<string>("UserName")
                        .HasMaxLength(30)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(30)")
                        .HasColumnName("USERNAME");

                    b.Property<long?>("switchStationStationId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("EmployeeId")
                        .HasName("SYS_C009095");

                    b.HasIndex("switchStationStationId");

                    b.ToTable("EMPLOYEE", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.Kpi", b =>
                {
                    b.Property<long>("KpiId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("KPI_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("KpiId"));

                    b.Property<double>("Score")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("SCORE");

                    b.Property<int>("ServiceFrequency")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("SERVICE_FREQUENCY");

                    b.Property<double>("TotalPerformance")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("TOTAL_PERFORMANCE");

                    b.Property<long>("employeeId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("KpiId")
                        .HasName("SYS_C009105");

                    b.HasIndex("employeeId")
                        .IsUnique();

                    b.ToTable("KPI", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.MaintenanceItem", b =>
                {
                    b.Property<long>("MaintenanceItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("MAINTENANCE_ITEM_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("MaintenanceItemId"));

                    b.Property<DateTime>("AppointTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("APPOINT_TIME");

                    b.Property<string>("Evaluation")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("MaintenanceLocation")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("MAINTENANCE_LOCATION");

                    b.Property<string>("Note")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("NOTE");

                    b.Property<int>("OrderStatus")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ORDER_STATUS");

                    b.Property<DateTime>("OrderSubmissionTime")
                        .HasPrecision(6)
                        .HasColumnType("TIMESTAMP(6)")
                        .HasColumnName("ORDER_SUBMISSION_TIME");

                    b.Property<double>("Score")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("SCORE");

                    b.Property<DateTime>("ServiceTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("SERVICE_TIME");

                    b.Property<string>("Title")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("TITLE");

                    b.Property<long>("VehicleId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long?>("VehicleOwnerOwnerId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<double>("latitude")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<double>("longitude")
                        .HasColumnType("BINARY_DOUBLE");

                    b.HasKey("MaintenanceItemId")
                        .HasName("SYS_C009117");

                    b.HasIndex("VehicleId");

                    b.HasIndex("VehicleOwnerOwnerId");

                    b.ToTable("MAINTENANCE_ITEM", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.News", b =>
                {
                    b.Property<long>("AnnouncementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("ANNOUNCEMENT_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AnnouncementId"));

                    b.Property<string>("Contents")
                        .HasMaxLength(2500)
                        .IsUnicode(true)
                        .HasColumnType("NCLOB")
                        .HasColumnName("CONTENTS");

                    b.Property<int>("Likes")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("LIKES");

                    b.Property<string>("PublishPos")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("PUBLISH_POS");

                    b.Property<DateTime>("PublishTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("PUBLISH_TIME");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("TITLE");

                    b.Property<int>("ViewCount")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("VIEW_COUNT");

                    b.Property<long>("administratorAdminId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("AnnouncementId")
                        .HasName("SYS_C009098");

                    b.HasIndex("administratorAdminId");

                    b.ToTable("NEWS", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.OwnerPos", b =>
                {
                    b.Property<long>("OwnerId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("Address")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("ADDRESS");

                    b.HasKey("OwnerId")
                        .HasName("SYS_C009099");

                    b.ToTable("OWNERPOS", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.SwitchLog", b =>
                {
                    b.Property<long>("SwitchServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("SWITCH_SERVICE_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("SwitchServiceId"));

                    b.Property<long>("EmployeeId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("Evaluation")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<double>("Score")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("SCORE");

                    b.Property<long>("SwitchRequestId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<DateTime>("SwitchTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("SWITCH_TIME");

                    b.Property<long>("VehicleId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("batteryOffBatteryId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("batteryOnBatteryId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("SwitchServiceId")
                        .HasName("SYS_C009138");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("SwitchRequestId");

                    b.HasIndex("VehicleId");

                    b.HasIndex("batteryOffBatteryId");

                    b.HasIndex("batteryOnBatteryId");

                    b.ToTable("SWITCH_LOG", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.SwitchRequest", b =>
                {
                    b.Property<long>("SwitchRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("SWITCH_REQUEST_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("SwitchRequestId"));

                    b.Property<long>("BatteryTypeId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<long>("EmployeeId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<double>("Latitude")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<double>("Longitude")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<string>("Note")
                        .HasMaxLength(255)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(255)")
                        .HasColumnName("NOTES");

                    b.Property<string>("Period")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Position")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("POSITION");

                    b.Property<int>("RequestStatus")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTime>("RequestTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("REQUEST_TIME");

                    b.Property<int>("SwitchType")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("SWITCH_TYPE");

                    b.Property<long>("VehicleId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("switchStationStationId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("vehicleOwnerOwnerId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("SwitchRequestId")
                        .HasName("SYS_C008772");

                    b.HasIndex("BatteryTypeId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("VehicleId");

                    b.HasIndex("switchStationStationId");

                    b.HasIndex("vehicleOwnerOwnerId");

                    b.ToTable("SWITCH_REQUEST", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.SwitchStation", b =>
                {
                    b.Property<long>("StationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("STATION_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("StationId"));

                    b.Property<string>("Address")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("AvailableBatteryCount")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("AVAILABLE_BATTERY_COUNT");

                    b.Property<int>("BatteryCapacity")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("BATTERY_CAPACITY");

                    b.Property<string>("City")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<float>("ElectricityFee")
                        .HasColumnType("BINARY_FLOAT");

                    b.Property<bool>("FailureStatus")
                        .HasColumnType("NUMBER(1)")
                        .HasColumnName("FAILURE_STATUS");

                    b.Property<double>("Latitude")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("LATITUDE");

                    b.Property<double>("Longitude")
                        .HasColumnType("BINARY_DOUBLE")
                        .HasColumnName("LONGITUDE");

                    b.Property<string>("ParkingFee")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("QueueLength")
                        .HasColumnType("NUMBER(10)");

                    b.Property<float>("ServiceFee")
                        .HasColumnType("BINARY_FLOAT")
                        .HasColumnName("SERVICE_FEE");

                    b.Property<string>("StationName")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("STATION_NAME");

                    b.Property<string>("Tags")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("TimeSpan")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("StationId")
                        .HasName("SYS_C009065");

                    b.ToTable("SWITCH_STATION", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.Vehicle", b =>
                {
                    b.Property<long>("VehicleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("VEHICLE_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("VehicleId"));

                    b.Property<long>("BatteryId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("PLATE_NUMBER");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("PURCHASE_DATE");

                    b.Property<long>("vehicleOwnerOwnerId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("vehicleParamVehicleModelId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("VehicleId")
                        .HasName("SYS_C009110");

                    b.HasIndex("BatteryId")
                        .IsUnique();

                    b.HasIndex("vehicleOwnerOwnerId");

                    b.HasIndex("vehicleParamVehicleModelId");

                    b.ToTable("VEHICLE", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.VehicleOwner", b =>
                {
                    b.Property<long>("OwnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("OWNER_ID");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("OwnerId"));

                    b.Property<string>("AccountSerial")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasColumnName("ACCOUNT_SERIAL");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("BIRTHDAY");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("CREATE_TIME");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("EMAIL");

                    b.Property<string>("Gender")
                        .HasMaxLength(3)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(3)")
                        .HasColumnName("GENDER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("PASSWORD");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(20)")
                        .HasColumnName("PHONE_NUMBER");

                    b.Property<byte[]>("ProfilePhoto")
                        .HasColumnType("BLOB")
                        .HasColumnName("PROFILE_PHOTO");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("USERNAME");

                    b.HasKey("OwnerId")
                        .HasName("SYS_C009088");

                    b.ToTable("VEHICLE_OWNER", "C##CAR");
                });

            modelBuilder.Entity("EntityFramework.Models.VehicleParam", b =>
                {
                    b.Property<long>("VehicleModelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)")
                        .HasColumnName("VEHICLE_MODEL");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("VehicleModelId"));

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("MANUFACTURER");

                    b.Property<int>("MaxSpeed")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("MAX_SPEED");

                    b.Property<string>("ModelName")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("ServiceTerm")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasColumnName("SERVICE_TERM");

                    b.Property<byte[]>("Sinp")
                        .HasColumnType("BLOB")
                        .HasColumnName("SINP");

                    b.Property<string>("Transmission")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(true)
                        .HasColumnType("NVARCHAR2(50)")
                        .HasColumnName("TRANSMISSION");

                    b.HasKey("VehicleModelId")
                        .HasName("SYS_C009081");

                    b.ToTable("VEHICLE_PARAM", "C##CAR");
                });

            modelBuilder.Entity("EmployeeMaintenanceItem", b =>
                {
                    b.HasOne("EntityFramework.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("employeesEmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.MaintenanceItem", null)
                        .WithMany()
                        .HasForeignKey("maintenanceItemsMaintenanceItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EntityFramework.Models.Battery", b =>
                {
                    b.HasOne("EntityFramework.Models.BatteryType", "batteryType")
                        .WithMany("batteries")
                        .HasForeignKey("BatteryTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.SwitchStation", "switchStation")
                        .WithMany("batteries")
                        .HasForeignKey("switchStationStationId");

                    b.Navigation("batteryType");

                    b.Navigation("switchStation");
                });

            modelBuilder.Entity("EntityFramework.Models.Employee", b =>
                {
                    b.HasOne("EntityFramework.Models.SwitchStation", "switchStation")
                        .WithMany("employees")
                        .HasForeignKey("switchStationStationId");

                    b.Navigation("switchStation");
                });

            modelBuilder.Entity("EntityFramework.Models.Kpi", b =>
                {
                    b.HasOne("EntityFramework.Models.Employee", "employee")
                        .WithOne("kpi")
                        .HasForeignKey("EntityFramework.Models.Kpi", "employeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("employee");
                });

            modelBuilder.Entity("EntityFramework.Models.MaintenanceItem", b =>
                {
                    b.HasOne("EntityFramework.Models.Vehicle", "vehicle")
                        .WithMany("maintenanceItems")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.VehicleOwner", null)
                        .WithMany("maintenanceItems")
                        .HasForeignKey("VehicleOwnerOwnerId");

                    b.Navigation("vehicle");
                });

            modelBuilder.Entity("EntityFramework.Models.News", b =>
                {
                    b.HasOne("EntityFramework.Models.Administrator", "administrator")
                        .WithMany("news")
                        .HasForeignKey("administratorAdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("administrator");
                });

            modelBuilder.Entity("EntityFramework.Models.OwnerPos", b =>
                {
                    b.HasOne("EntityFramework.Models.VehicleOwner", "vehicleowner")
                        .WithMany("ownerpos")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("vehicleowner");
                });

            modelBuilder.Entity("EntityFramework.Models.SwitchLog", b =>
                {
                    b.HasOne("EntityFramework.Models.Employee", "employee")
                        .WithMany("switchLogs")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.SwitchRequest", "switchrequest")
                        .WithMany()
                        .HasForeignKey("SwitchRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Vehicle", "vehicle")
                        .WithMany("SwitchLogs")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Battery", "batteryOff")
                        .WithMany("switchLogsOff")
                        .HasForeignKey("batteryOffBatteryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Battery", "batteryOn")
                        .WithMany("switchLogsOn")
                        .HasForeignKey("batteryOnBatteryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("batteryOff");

                    b.Navigation("batteryOn");

                    b.Navigation("employee");

                    b.Navigation("switchrequest");

                    b.Navigation("vehicle");
                });

            modelBuilder.Entity("EntityFramework.Models.SwitchRequest", b =>
                {
                    b.HasOne("EntityFramework.Models.BatteryType", "batteryType")
                        .WithMany()
                        .HasForeignKey("BatteryTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Employee", "employee")
                        .WithMany("switchRequests")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.Vehicle", "vehicle")
                        .WithMany("switchRequests")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.SwitchStation", "switchStation")
                        .WithMany("switchRequests")
                        .HasForeignKey("switchStationStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.VehicleOwner", "vehicleOwner")
                        .WithMany("switchRequests")
                        .HasForeignKey("vehicleOwnerOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("batteryType");

                    b.Navigation("employee");

                    b.Navigation("switchStation");

                    b.Navigation("vehicle");

                    b.Navigation("vehicleOwner");
                });

            modelBuilder.Entity("EntityFramework.Models.Vehicle", b =>
                {
                    b.HasOne("EntityFramework.Models.Battery", "Battery")
                        .WithOne("vehicle")
                        .HasForeignKey("EntityFramework.Models.Vehicle", "BatteryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.VehicleOwner", "vehicleOwner")
                        .WithMany("vehicles")
                        .HasForeignKey("vehicleOwnerOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EntityFramework.Models.VehicleParam", "vehicleParam")
                        .WithMany("vehicles")
                        .HasForeignKey("vehicleParamVehicleModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Battery");

                    b.Navigation("vehicleOwner");

                    b.Navigation("vehicleParam");
                });

            modelBuilder.Entity("EntityFramework.Models.Administrator", b =>
                {
                    b.Navigation("news");
                });

            modelBuilder.Entity("EntityFramework.Models.Battery", b =>
                {
                    b.Navigation("switchLogsOff");

                    b.Navigation("switchLogsOn");

                    b.Navigation("vehicle");
                });

            modelBuilder.Entity("EntityFramework.Models.BatteryType", b =>
                {
                    b.Navigation("batteries");
                });

            modelBuilder.Entity("EntityFramework.Models.Employee", b =>
                {
                    b.Navigation("kpi")
                        .IsRequired();

                    b.Navigation("switchLogs");

                    b.Navigation("switchRequests");
                });

            modelBuilder.Entity("EntityFramework.Models.SwitchStation", b =>
                {
                    b.Navigation("batteries");

                    b.Navigation("employees");

                    b.Navigation("switchRequests");
                });

            modelBuilder.Entity("EntityFramework.Models.Vehicle", b =>
                {
                    b.Navigation("SwitchLogs");

                    b.Navigation("maintenanceItems");

                    b.Navigation("switchRequests");
                });

            modelBuilder.Entity("EntityFramework.Models.VehicleOwner", b =>
                {
                    b.Navigation("maintenanceItems");

                    b.Navigation("ownerpos");

                    b.Navigation("switchRequests");

                    b.Navigation("vehicles");
                });

            modelBuilder.Entity("EntityFramework.Models.VehicleParam", b =>
                {
                    b.Navigation("vehicles");
                });
#pragma warning restore 612, 618
        }
    }
}
