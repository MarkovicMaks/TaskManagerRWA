using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class UserSkill
{
    public int? UserId { get; set; }

    public int? SkillId { get; set; }

    public virtual Skill? Skill { get; set; }

    public virtual User? User { get; set; }
}
