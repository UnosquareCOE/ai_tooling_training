using Game.Services.Dto;

namespace Game.Services.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGame(string language);
        GameDto? GetGame(Guid gameId);
        MakeGuessDto MakeGuess(Guid gameId, string letter);
        bool DeleteGame(Guid gameId);
    }
}