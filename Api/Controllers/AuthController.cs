using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Try to find the user by username (or use email if you prefer)
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Verify password
            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(default!, user.PasswordHash, loginDto.Password);
            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Get JWT settings
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT signing key is missing! Set Jwt__Key as an environment variable or in .env.");

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Build token
            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(jwtSettings["ExpiresInMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // Return token to client
            return Ok(new { token = jwt });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Ok(new { message = "If that email exists, a reset token has been issued." });

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = expiresAt
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // Check environment
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            bool isDev = string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase)
                || string.Equals(env, "Local", StringComparison.OrdinalIgnoreCase);

            if (isDev)
            {
                // Show token in dev/local
                return Ok(new
                {
                    message = "If that email exists, a reset token has been issued.",
                    resetToken = token
                });
            }
            else
            {
                // Hide token in staging/prod
                return Ok(new
                {
                    message = "If that email exists, a reset token has been issued."
                });
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == request.Token && !t.Used && t.ExpiresAt > DateTime.UtcNow);

            if (resetToken?.User == null)
                return BadRequest(new { message = "Invalid or expired reset token." });

            var hasher = new PasswordHasher<object>();
            resetToken.User.PasswordHash = hasher.HashPassword(default!, request.NewPassword);

            resetToken.Used = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
