using Microsoft.EntityFrameworkCore;

namespace Game.DAL.Interfaces;

public interface IGameContext
{
    DbSet<Models.Game> Games { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}