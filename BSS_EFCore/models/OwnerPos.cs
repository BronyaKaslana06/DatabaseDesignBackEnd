using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class OwnerPos
{
    public long OwnerId { get; set; }
    public string? Address { get; set; }
    public VehicleOwner vehicleowner { get; set; }
}
