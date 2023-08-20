using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public partial class Battery
{
    public long BatteryId { get; set; }

    public int? AvailableStatus { get; set; }

    [NotMapped]
    public AvailableStatusEnum AvailableStatusEnum
    {
        get
        {
            if (AvailableStatus == null || AvailableStatus <= 0 || AvailableStatus >= 5)
                AvailableStatus = 4;
            return (AvailableStatusEnum)AvailableStatus;
        }
        set
        {
            AvailableStatus = (int)value;
        }
    }

    public double CurrentCapacity { get; set; } 

    public int CurrChargeTimes { get; set; }

    public DateTime ManufacturingDate { get; set; }

    public SwitchStation? switchStation { get; set; }   //nullable
    public BatteryType batteryType { get; set; }   //非空
    public Vehicle? vehicle { get; set; }   //nullable
    public List<SwitchLog> switchLogsOn { get; set; } = new List<SwitchLog>(); //nullable
    public List<SwitchLog> switchLogsOff { get; set; } = new List<SwitchLog>();  //nullable
    
}


public enum AvailableStatusEnum
{
    available = 1,
    onCar = 2,
    discharged = 3,
    Unknown = 4
}