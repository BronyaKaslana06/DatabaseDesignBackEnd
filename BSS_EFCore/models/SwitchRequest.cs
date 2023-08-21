using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public partial class SwitchRequest
{
    public long SwitchRequestId { get; set; }

    public int SwitchType { get; set; }

    [NotMapped]
    //[Newtonsoft.Json.JsonIgnore]
    public SwitchTypeEnum SwitchTypeEnum
    {
        get
        {
            switch (SwitchType)
            {
                case (int)SwitchTypeEnum.inPerson:
                    return SwitchTypeEnum.inPerson;
                case (int)SwitchTypeEnum.not_inPerson:
                    return SwitchTypeEnum.not_inPerson;
                default:
                    return SwitchTypeEnum.Unknown;
            }
        }
        set
        {
            SwitchType = (int)value;
        }
    }


    public DateTime RequestTime { get; set; }

    public string? Position { get; set; }
    public double Longtitue { get; set; }
    public double Latitude { get; set; }
    public string? Note { get; set; }


    public Employee employee { get; set; }  //非空
    public VehicleOwner vehicleOwner { get; set; }  //非空
    public Vehicle vehicle { get; set; } //非空
}

public enum SwitchTypeEnum
{
    inPerson = 1,
    not_inPerson = 2,
    Unknown = 3
}