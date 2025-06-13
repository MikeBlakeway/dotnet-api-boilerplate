namespace Api.Models
{
    public class User
    {
        public int Id { get; set; }              // Primary key
        public required string Username { get; set; }     // Required, unique
        public required string Email { get; set; }        // Required, unique
        public required string PasswordHash { get; set; } // Store hashed passwords (never plain text!)
        public required string Role { get; set; }         // e.g. "User", "Admin"
    }
}
