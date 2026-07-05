using System.ComponentModel.DataAnnotations;

namespace DTO.User
{

    public class SignUp
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 30 characters.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(30, ErrorMessage = "Password cannot exceed 30 characters.")]
        public required string Password { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class Login
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }

}