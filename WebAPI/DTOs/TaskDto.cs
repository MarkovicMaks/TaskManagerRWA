using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Manager ID is required")]
        public int? ManagerId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; }

        public string Description { get; set; }
        
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }
    }
}
