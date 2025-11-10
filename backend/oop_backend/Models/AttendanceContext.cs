using Microsoft.EntityFrameworkCore;

namespace oop_backend.Models
{
    public class AttendanceContext : DbContext
    {
        public AttendanceContext(DbContextOptions<AttendanceContext> options) : base(options)
        {
        }

        public DbSet<Attendance> Attendance { get; set; }
    }
}
