using dal.Interfaces;
using dal.Models;
using Microsoft.EntityFrameworkCore;

namespace dal.Contexts;

public class GameContext : DbContext, IGameContext
{
    public DbSet<Game> Games { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");
        }
    }
}