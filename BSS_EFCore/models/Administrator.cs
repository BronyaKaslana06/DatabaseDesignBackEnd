using System;
using System.Collections.Generic;

namespace EntityFramework.Models;

public partial class Administrator
{
    public long AdminId { get; set; } 
    public string Email { get; set; }
    public string Password { get; set; }
    public List<News> news { get; set; } = new List<News>();

}
