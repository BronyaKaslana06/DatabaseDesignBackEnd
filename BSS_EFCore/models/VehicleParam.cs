using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace EntityFramework.Models;

public partial class VehicleParam
{
    public long VehicleModelId { get; set; }

    public string ModelName { get; set; }

    public string Transmission { get; set; }

    public DateTime ServiceTerm { get; set; }

    public string Manufacturer { get; set; }

    public int MaxSpeed { get; set; }

    public byte[]? Sinp { get; set; }

    public virtual List<Vehicle> vehicles { get; set; } = new List<Vehicle>();  //至少为1
}
