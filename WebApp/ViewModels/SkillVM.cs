using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class SkillVM
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public List<int> TaskSkillIds { get; set; } = new List<int>();
        public List<int> UserSkillIds { get; set; } = new List<int>();
    }
}
