using System;

namespace Api.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; } = false;

        public User? User { get; set; }
    }
}
