namespace WebApp.ViewModels
{
    public class TaskAssignmentVM
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string? TaskName { get; set; }
        public string? UserName { get; set; }
    }
}
