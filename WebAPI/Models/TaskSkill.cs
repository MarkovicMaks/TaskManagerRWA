using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class TaskSkill
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int SkillId { get; set; }

    public virtual Skill Skill { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
