using Game.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Game.DAL.Contexts;

public class GameContext : DbContext, IGameContext
{
    public DbSet<Models.Game> Games { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");
        }
    }
}