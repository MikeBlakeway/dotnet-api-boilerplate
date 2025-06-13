using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = default!;

        [Required]
        [RegularExpression("User|Admin", ErrorMessage = "Role must be 'User' or 'Admin'.")]
        public string Role { get; set; } = default!;
    }
}
