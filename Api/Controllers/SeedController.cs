using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/seed/admin
        // Bootstrap ONLY: allows creation of first admin user, only if no users exist, and only in development.
        [HttpPost("admin")]
        public IActionResult SeedAdmin([FromBody] SeedAdminRequest request)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid("Seeding is only allowed in the Development environment.");
            }

            if (_context.Users.Any())
            {
                return Forbid("Admin user can only be created if there are no users in the database.");
            }

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return BadRequest("A strong password (at least 6 characters) is required.");
            }

            var added = DbSeeder.SeedAdminUser(_context, request.Password);

            return Ok(new { message = $"Admin user created.", username = "admin" });
        }

        // POST: api/seed/users
        // Bulk user seeding is always restricted: requires admin auth and dev env.
        [Authorize(Roles = "Admin")]
        [HttpPost("users")]
        public IActionResult SeedUsers([FromBody] SeedUsersRequest request)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.Equals(env, "Development", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid("Seeding is only allowed in the Development environment.");
            }

            if (request.Count < 1 || request.Count > 1000)
                return BadRequest("Count must be between 1 and 1000.");

            var added = DbSeeder.SeedUsers(_context, request.Count, request.Roles);
            return Ok(new { message = $"{added} users seeded." });
        }
    }
}
