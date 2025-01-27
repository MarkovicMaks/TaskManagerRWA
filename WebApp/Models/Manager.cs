using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Manager
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual User? User { get; set; }
}
