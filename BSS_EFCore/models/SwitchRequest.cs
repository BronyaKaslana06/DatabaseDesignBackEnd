using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                case (int)SwitchTypeEnum.到店换电:
                    return SwitchTypeEnum.到店换电;
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

    [Obsolete]
    public int Period { get; set; }

    [NotMapped]
    public PeriodEnum PeriodEnum
    {
        get
        {
            if (Period <= 0 || Period > 7)
                Period = 7;
            return (PeriodEnum)Period;
        }
        set
        {
            Period = (int)value;
        }
    }

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
    public Vehicle vehicle { get; set; } //非空
    public BatteryType batteryType { get; set; } //非空
    public SwitchLog switchLog { get; set; }   //nullable
}

public enum PeriodEnum
{
    [Display(Name = "00:00-04:00")]
    时间段00_00_04_00 = 1,

    [Display(Name = "04:00-08:00")]
    时间段04_00_08_00 = 2,

    [Display(Name = "08:00-12:00")]
    时间段08_00_12_00 = 3,

    [Display(Name = "12:00-16:00")]
    时间段12_00_16_00 = 4,

    [Display(Name = "16:00-20:00")]
    时间段16_00_20_00 = 5,

    [Display(Name = "20:00-24:00")]
    时间段20_00_24_00 = 6,

    [Display(Name = "时间段非法")]
    时间段非法 = 7
}
public enum SwitchTypeEnum
{
    未知 = 0,
    上门换电 = 1,
    到店换电 = 2
}

public enum RequestStatusEnum
{
    未知 = 0,
    待接单 = 1,
    待完成 = 2,
    待评价 = 3,
    已完成 = 4
}