using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class TaskBase
    {
        [Required(ErrorMessage = "Task Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Invalid Title, title must me 3 - 100 characters long")]
        public required string Title { get; set; }
        public string? Description { get; set; } = "No description";

        [Range(0, 100, ErrorMessage = "Progress should be in between 0 to 100")]
        public int progress { get; set; } = 0;
    }

    public class AddTask : TaskBase
    {
        [Required(ErrorMessage = "User Id is required")]
        public required int UserId { get; set; }

        [Required(ErrorMessage = "Category Id is required")]
        public required int CategoryId { get; set; }
    }

    public class EditTask : TaskBase
    {
        [Required(ErrorMessage = "Task id is required to edit a task")]
        public required int Id { get; set; }

        public bool isActive { get; set; } = true;
    }
}