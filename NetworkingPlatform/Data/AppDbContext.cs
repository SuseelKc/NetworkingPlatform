using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {


        }
        public DbSet<Users> Users { get; set; }
    }
}
