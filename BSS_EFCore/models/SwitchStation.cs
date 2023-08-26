using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class SwitchStation
{
    public long StationId { get; set; } 

    public string? StationName { get; set; }

    public float ServiceFee { get; set; }

    public float ElectricityFee { get; set; }

    public string ParkingFee { get; set; }

    public int QueueLength { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public bool FailureStatus { get; set; }

    public int BatteryCapacity { get; set; }

    public int AvailableBatteryCount { get; set; }

    public string? City { get; set; }

    public string? Tags { get; set; }

    public string? Address { get; set; }

    public string? TimeSpan { get; set; }

    public List<Employee> employees { get; set; } = new List<Employee>();  //至少一个
    public List<Battery> batteries { get; set; } = new List<Battery>();   //可以为0
    public List<SwitchRequest> switchRequests { get; set; } = new List<SwitchRequest>(); //可以为0

}
