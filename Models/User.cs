using Microsoft.EntityFrameworkCore;

namespace UserManagement.Models
{
    // User-modellen, der afspejler SQL-tabellen Users
    public class User
    {
        public int Id { get; set; } // Primær nøgle
        public string FirstName { get; set; } = string.Empty; // NOT NULL
        public string LastName { get; set; } = string.Empty; // NOT NULL
        public string Email { get; set; } = string.Empty; // NOT NULL
        public string UserName { get; set; } = string.Empty; // NOT NULL
        public string PassWord { get; set; } = string.Empty; // NOT NULL
        public int? AccessLevel { get; set; } // Kan være NULL
    }

    // DbContext til User-tabellen
    public class UserDb : DbContext
    {
        public UserDb(DbContextOptions options) : base(options) { }

        // DbSet til at interagere med Users-tabellen i databasen
        public DbSet<User> Users { get; set; } = null!;
    }
}
