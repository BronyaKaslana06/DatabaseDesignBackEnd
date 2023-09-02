using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class News
{
    public long AnnouncementId { get; set; }

    public DateTime PublishTime { get; set; }

    public string? PublishPos { get; set; }

    public string? Title { get; set; }

    public string? Contents { get; set; }

    public int Likes { get; set; }

    public int ViewCount { get; set; }
    public Administrator administrator { get; set; } 
}
