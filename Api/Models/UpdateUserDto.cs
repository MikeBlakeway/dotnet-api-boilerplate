using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [RegularExpression("User|Admin", ErrorMessage = "Role must be 'User' or 'Admin'.")]
        public string Role { get; set; } = default!;
    }
}
