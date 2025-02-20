﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class TaskVM
    {
        public int Id { get; set; }
        [Display(Name = "Select Manager")]
        public int? ManagerId { get; set; }
        [Display(Name = "Manager")]
        [ValidateNever]
        public string? ManagerName { get; set; }

        [Display(Name = "Task Name")]
        [Required(ErrorMessage = "There's not much sense of having a task without the knowing what it is, right?")]
        public string? Title { get; set; }

        public string? Description { get; set; }
        
        public DateTime? CreatedAt { get; set; }

        public string? Status { get; set; }

        public List<int> SelectedSkillIds { get; set; } = new();
        [Display(Name = "Skills")]
        [ValidateNever]
        public List<string> AssignedSkills { get; set; } = new();
        public IEnumerable<SelectListItem> SkillOptions { get; set; } = new List<SelectListItem>();
    }
}
