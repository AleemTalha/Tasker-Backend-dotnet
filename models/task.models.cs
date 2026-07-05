using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ToDoTask
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required, MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = "";

        public required int CategoryId { get; set; }

        public Category? category { get; set; } = null;
        public required int UserId { get; set; }
        public User user { get; set; } = null!;

        [Range(0, 100, ErrorMessage = "Progress should be in between 0 to 100")]
        public int progress { get; set; } = 0;

        public bool isActive { get; set; } = true;

    }
}