using System;
using System.Collections.Generic;

namespace TM.BL.Models;

public partial class Manager
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Task> Tasks { get; } = new List<Task>();

    public virtual User? User { get; set; }
}
