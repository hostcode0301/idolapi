using idolapi.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace idolapi.DB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Idol> Idols { get; set; }
    }
}