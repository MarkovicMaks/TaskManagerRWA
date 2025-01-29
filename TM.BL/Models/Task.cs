using System;
using System.Collections.Generic;

namespace TM.BL.Models;

public partial class Task
{
    public int Id { get; set; }

    public int? ManagerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Manager? Manager { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; } = new List<TaskAssignment>();

    public virtual ICollection<TaskSkill> TaskSkills { get; } = new List<TaskSkill>();
}
