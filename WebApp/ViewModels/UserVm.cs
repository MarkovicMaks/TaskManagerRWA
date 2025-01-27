using WebApp.Models;

namespace WebApp.ViewModels
{
    public class UserVm
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public List<TaskAssignment>? TaskAssignments { get; set; }
    }
}
