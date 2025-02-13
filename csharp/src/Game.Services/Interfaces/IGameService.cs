using Game.Services.Dto;

namespace Game.Services.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGameAsync(string language);
        Task<GameDto?> GetGameAsync(Guid gameId);
        Task<MakeGuessDto?>  MakeGuess(Guid gameId, string letter);
        Task<bool>  DeleteGame(Guid gameId);
    }
}