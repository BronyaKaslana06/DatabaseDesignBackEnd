using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace EntityFramework.Models;

public partial class MaintenanceItem
{
    public long MaintenanceItemId { get; set; } 

    public string MaintenanceLocation { get; set; }

    public string? Note { get; set; }

    public string? Title { get; set; }

    public DateTime ServiceTime { get; set; }

    public DateTime OrderSubmissionTime { get; set; }

    public DateTime AppointTime { get; set; }

    [Obsolete]
    public int OrderStatus { get; set; }

    [NotMapped]
    //[Newtonsoft.Json.JsonIgnore]
    public OrderStatusEnum OrderStatusEnum
    {
        get
        {
            switch (OrderStatus)
            {
                case (int)OrderStatusEnum.unhandled:
                    return OrderStatusEnum.unhandled;
                case (int)OrderStatusEnum.handling:
                    return OrderStatusEnum.handling;
                case (int)OrderStatusEnum.finish:
                    return OrderStatusEnum.finish;
                default:
                    return OrderStatusEnum.Unknown;
            }
        }
        set
        {
            OrderStatus = (int)value;
        }
    }


    public double Score { get; set; }

    public List<Employee> employees = new List<Employee>();   //至少一个
    public Vehicle vehicle { get; set; }   // not null
}


public enum OrderStatusEnum
{
    unhandled = 1,
    handling = 2,
    finish = 3,
    Unknown=4
}
