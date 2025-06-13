using Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Api.Data
{
    public static class DbSeeder
    {
        public static List<User> GenerateUsers(int count, string[]? roles = null)
        {
            var users = new List<User>();
            var hasher = new PasswordHasher<object>();
            var defaultRoles = roles ?? new[] { "User", "Admin", "Manager" };
            var rand = new Random();

            for (int i = 1; i <= count; i++)
            {
                var role = defaultRoles[rand.Next(defaultRoles.Length)];
                var username = $"user{i}";
                var email = $"user{i}@example.com";
                var password = $"Password{i}!";
                users.Add(new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = hasher.HashPassword(default!, password),
                    Role = role
                });
            }
            return users;
        }

        public static int SeedUsers(AppDbContext context, int count, string[]? roles = null)
        {
            var users = GenerateUsers(count, roles);
            // Don't duplicate usernames/emails
            var newUsers = users.Where(u => !context.Users.Any(dbU => dbU.Username == u.Username)).ToList();
            context.Users.AddRange(newUsers);
            context.SaveChanges();
            return newUsers.Count;
        }

        public static int SeedAdminUser(AppDbContext context, string password)
        {
            // If any users exist, don't allow seeding
            if (context.Users.Any())
                return 0;

            var hasher = new PasswordHasher<object>();
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@notarealemail.com",
                PasswordHash = hasher.HashPassword(default!, password),
                Role = "Admin"
            };
            context.Users.Add(adminUser);
            context.SaveChanges();
            return 1;
        }
    }
}
