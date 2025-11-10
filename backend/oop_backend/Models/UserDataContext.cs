using Microsoft.EntityFrameworkCore;
namespace oop_backend.Models
{
    public class UserDataContext: DbContext
    {
        public UserDataContext(DbContextOptions<UserDataContext> options) : base(options)
        {
        }

        public DbSet<UserData> Users { get; set; }
    }
}
