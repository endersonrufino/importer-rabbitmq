using Microsoft.EntityFrameworkCore;
using shared.Entities;

namespace shared.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<FileUpload> Files { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
