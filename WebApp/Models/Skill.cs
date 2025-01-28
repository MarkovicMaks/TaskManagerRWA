using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TaskSkill> TaskSkills { get; set; } = new List<TaskSkill>();

    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
