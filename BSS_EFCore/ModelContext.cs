using System;
using System.Collections.Generic;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EntityFramework.Context;

public class ModelContext : DbContext
{
    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Battery> Batteries { get; set; }


    public virtual DbSet<BatteryType> BatteryTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }


    public virtual DbSet<Kpi> Kpis { get; set; }

    public virtual DbSet<MaintenanceItem> MaintenanceItems { get; set; }


    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<SwitchLog> SwitchLogs { get; set; }

    public virtual DbSet<SwitchRequest> SwitchRequests { get; set; }


    public virtual DbSet<SwitchStation> SwitchStations { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleOwner> VehicleOwners { get; set; }

    public virtual DbSet<OwnerPos> OwnerPos { get; set; }

    public virtual DbSet<VehicleParam> VehicleParams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    {
        //optionsBuilder.LogTo(Console.WriteLine);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##CAR")
            .UseCollation("USING_NLS_COMP");

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }

}
