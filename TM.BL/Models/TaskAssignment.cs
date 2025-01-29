using System;
using System.Collections.Generic;

namespace TM.BL.Models;

public partial class TaskAssignment
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
