using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public required string FullName { get; set; }

        [Required, MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public required string Email { get; set; }

        [Required, MaxLength(225, ErrorMessage = "Password cannot exceed 100 characters.")]
        public required string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public UserRole UserRole { get; set; } = UserRole.User;

        public ICollection<ToDoTask> Tasks { get; set; } = new List<ToDoTask>();

    }
    public enum UserRole
    {
        Admin,
        User
    }
}