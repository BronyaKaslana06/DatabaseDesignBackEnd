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

    public double longitude { get; set; }

    public double latitude { get; set; }

    public DateTime? ServiceTime { get; set; }

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
                case (int)OrderStatusEnum.待接单:
                    return OrderStatusEnum.待接单;
                case (int)OrderStatusEnum.待完成:
                    return OrderStatusEnum.待完成;
                case (int)OrderStatusEnum.待评分:
                    return OrderStatusEnum.待评分;
                case (int)OrderStatusEnum.已完成:
                    return OrderStatusEnum.已完成;
                default:
                    return OrderStatusEnum.未知;
            }
        }
        set
        {
            OrderStatus = (int)value;
        }
    }


    public double Score { get; set; }

    public string? Evaluation { get; set; }

    public List<Employee> employees = new List<Employee>();   //可以0个
    public Vehicle vehicle { get; set; }   // not null
}


public enum OrderStatusEnum
{
    未知 = 0,
    待接单 = 1,
    待完成 = 2,
    待评分 = 3,
    已完成 = 4
}
