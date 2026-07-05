using System.ComponentModel.DataAnnotations;

namespace DTO.Category
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Category name must be between 3 and 50 characters.")]
        public required string Name { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }
    }
}