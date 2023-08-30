using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public partial class SwitchRequest
{
    public long SwitchRequestId { get; set; }

    [Obsolete]
    public int SwitchType { get; set; }

    [NotMapped]
    //[Newtonsoft.Json.JsonIgnore]
    public SwitchTypeEnum SwitchTypeEnum
    {
        get
        {
            switch (SwitchType)
            {
                case (int)SwitchTypeEnum.上门换电:
                    return SwitchTypeEnum.上门换电;
                case (int)SwitchTypeEnum.预约换电:
                    return SwitchTypeEnum.预约换电;
                default:
                    return SwitchTypeEnum.未知;
            }
        }
        set
        {
            SwitchType = (int)value;
        }
    }

    public DateTime RequestTime { get; set; }

    public string? Position { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string? Note { get; set; }

    public DateTime Date { get; set; }
    public string? Period { get; set; }

    [Obsolete]
    public int RequestStatus { get; set; }
    [NotMapped]
    public RequestStatusEnum requestStatusEnum
    {
        get
        {
            switch (RequestStatus)
            {
                case (int)RequestStatusEnum.待接单:
                    return RequestStatusEnum.待接单;
                case (int)RequestStatusEnum.待完成:
                    return RequestStatusEnum.待完成;
                case (int)RequestStatusEnum.待评价:
                    return RequestStatusEnum.待评价;
                case (int)RequestStatusEnum.已完成:
                    return RequestStatusEnum.已完成;
                default:
                    return RequestStatusEnum.未知;
            }
        }
        set
        {
            RequestStatus = (int)value;
        }
    }

    public Employee employee { get; set; }  //非空
    public VehicleOwner vehicleOwner { get; set; }  //非空
    public Vehicle vehicle { get; set; } //非空
    public SwitchStation switchStation { get; set; } //非空
    public BatteryType batteryType { get; set; } //非空
    public SwitchLog switchLog { get; set; }   //nullable
}

public enum SwitchTypeEnum
{
    未知 = 0,
    上门换电 = 1,
    预约换电 = 2
}

public enum RequestStatusEnum
{
    未知 = 0,
    待接单 = 1,
    待完成 = 2,
    待评价 = 3,
    已完成 = 4
}