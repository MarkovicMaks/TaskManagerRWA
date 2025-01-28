using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Task
{
    public int Id { get; set; }

    public int? ManagerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public byte[] CreatedAt { get; set; } = null!;

    public string? Status { get; set; }

    public virtual Manager? Manager { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskSkill> TaskSkills { get; set; } = new List<TaskSkill>();
}
