using Microsoft.EntityFrameworkCore;

namespace dal
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");
        }
    }
}