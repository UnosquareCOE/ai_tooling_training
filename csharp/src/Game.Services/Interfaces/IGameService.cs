using Game.Services.Dto;

namespace Game.Services.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGameAsync(string language);
        Task<GameDto?> GetGameAsync(Guid gameId);
        MakeGuessDto MakeGuess(Guid gameId, string letter);
        bool DeleteGame(Guid gameId);
    }
}