using System;
using System.Collections.Generic;

namespace webapi.Models;

public partial class MaintenanceItem
{
    public string MaintenanceItemId { get; set; } = null!;

    public string VehicleId { get; set; } = null!;

    public string? MaintenanceLocation { get; set; }
    public string? Title { get; set; }

    public string? Remarks { get; set; }

    public DateTime ServiceTime { get; set; }

    public DateTime? OrderSubmissionTime { get; set; }

    public string? OrderStatus { get; set; }

    public string? Evaluations { get; set; }

    public virtual MaintenanceItemEmployee? MaintenanceItemEmployee { get; set; }

    public virtual Vehicle Vehicle { get; set; } = null!;
}
