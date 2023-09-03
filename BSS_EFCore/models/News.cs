using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.Models;

public partial class News
{
    public long AnnouncementId { get; set; }

    public DateTime PublishTime { get; set; }

    [Obsolete]
    public int PublishPos { get; set; }
    [NotMapped]
    public PublishPosEnum AvailableStatusEnum
    {
        get
        {
            if (PublishPos == null || PublishPos <= 0 || PublishPos >= 4)
                PublishPos = 3;
            return (PublishPosEnum)PublishPos;
        }
        set
        {
            PublishPos = (int)value;
        }
    }

    public string? Title { get; set; }

    public string? Contents { get; set; }

    public int Likes { get; set; }

    public int ViewCount { get; set; }
    public Administrator administrator { get; set; } 
}


public enum PublishPosEnum
{
    员工 = 1,
    用户 = 2,
    未知 = 3
}