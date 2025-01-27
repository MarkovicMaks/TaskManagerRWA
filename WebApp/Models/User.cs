using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Role { get; set; }

    public string PwdHash { get; set; } = null!;

    public string PwdSalt { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual Manager? Manager { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}
