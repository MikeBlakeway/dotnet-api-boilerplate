namespace Api.Models
{
    public class SeedUsersRequest
    {
        public int Count { get; set; } = 10; // default number of users
        public string[]? Roles { get; set; } // e.g., ["User", "Admin"]
    }
}
