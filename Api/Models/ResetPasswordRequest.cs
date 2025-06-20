namespace Api.Models
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
