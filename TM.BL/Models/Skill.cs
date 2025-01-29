using System;
using System.Collections.Generic;

namespace TM.BL.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TaskSkill> TaskSkills { get; } = new List<TaskSkill>();

    public virtual ICollection<UserSkill> UserSkills { get; } = new List<UserSkill>();
}
