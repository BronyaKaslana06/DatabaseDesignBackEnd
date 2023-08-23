using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class Vehicle
{
    public long VehicleId { get; set; } 

    public DateTime PurchaseDate { get; set; }

    public long BatteryId { get; set; }

    public string? PlateNumber { get; set; }

    public Battery Battery { get; set; }   //非空

    public List<MaintenanceItem> maintenanceItems { get; set; } = new List<MaintenanceItem>();  //可以为0
    public List<SwitchRequest> switchRequests { get; set; } = new List<SwitchRequest>();  //可以为0

    public VehicleOwner vehicleOwner { get; set; }   //非空

    public List<SwitchLog> SwitchLogs { get; set; } = new List<SwitchLog>();  //可以为0

    public VehicleParam vehicleParam { get; set; }   //非空
}
