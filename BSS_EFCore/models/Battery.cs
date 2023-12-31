﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public partial class Battery
{
    public long BatteryId { get; set; }


    [Obsolete]
    public int? AvailableStatus { get; set; }

    [NotMapped]
    public AvailableStatusEnum AvailableStatusEnum
    {
        get
        {
            if (AvailableStatus == null || AvailableStatus <= 0 || AvailableStatus >= 7)
                AvailableStatus = 6;
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
    可用 = 1,
    已预定 = 2,
    汽车使用中 = 3,
    充电中 = 4,
    损坏 = 5,
    未知 = 6
}