using dal.Models;
using Microsoft.EntityFrameworkCore;

namespace dal.Interfaces
{
    public interface IGameContext
    {
        DbSet<Game> Games { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}