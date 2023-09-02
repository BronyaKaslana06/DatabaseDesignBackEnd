using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class Kpi
{
    public long KpiId { get; set; }

    public double TotalPerformance { get; set; }

    public int ServiceFrequency { get; set; }

    public double Score { get; set; }

    public Employee employee { get; set; } 
    public long employeeId { get; set; }
}
