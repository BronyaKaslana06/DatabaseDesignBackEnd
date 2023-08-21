using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class SwitchLog
{
    public long SwitchServiceId { get; set; } 

    public DateTime SwitchTime { get; set; }

    public double Score { get; set; }

    public Battery batteryOn { get; set; } //非空

    public Battery batteryOff { get; set; } //非空

    public Vehicle vehicle { get; set; }   //非空

    public Employee employee { get; set; }    //not null

    public SwitchLog switchlog { get; set; }    //not null
}
