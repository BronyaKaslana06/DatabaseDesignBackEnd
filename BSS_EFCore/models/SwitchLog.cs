using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class SwitchLog
{
    public long SwitchServiceId { get; set; } 

    public DateTime SwitchTime { get; set; }

    public double Score { get; set; }

    public string Evaluation { get; set; }





    public Battery batteryOn { get; set; } //非空

    public Battery batteryOff { get; set; } //非空

    public Vehicle vehicle { get; set; }   //非空

    public Employee employee { get; set; }    //not null

    public long switchRequestId { get; set; }
    public SwitchRequest switchrequest { get; set; }    //not null
}
