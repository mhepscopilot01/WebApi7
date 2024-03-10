using Microsoft.EntityFrameworkCore;
using WebApi7.Models;

// This class is used to connect to the database and to map the User model to a table in the database
namespace WebApi7.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { } // Constructor

        public DbSet<User> Users { get; set; } // This is the table in the database
    }
}
