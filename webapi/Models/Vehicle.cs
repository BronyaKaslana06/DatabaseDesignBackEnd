using System;
using System.Collections.Generic;

namespace webapi.Models;

public partial class Vehicle
{
    public string VehicleId { get; set; } = null!;

    public string VehicleModel { get; set; } = null!;

    public string OwnerId { get; set; } = null!;

    public string PlateNumber { get; set; } = null!;

    public DateTime PurchaseDate { get; set; }

    public string? BatteryId { get; set; }

    public virtual Battery? Battery { get; set; }

    public virtual ICollection<MaintenanceItem> MaintenanceItems { get; set; } = new List<MaintenanceItem>();

    public virtual VehicleOwner Owner { get; set; } = null!;

    public virtual ICollection<SwitchLog> SwitchLogs { get; set; } = new List<SwitchLog>();

    public virtual VehicleParam VehicleModelNavigation { get; set; } = null!;
}
