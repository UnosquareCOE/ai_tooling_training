using Game.Services.Dto;

namespace Game.Services.Interfaces
{
    public interface IGameService
    {
        Guid CreateGame();
        GameDto? GetGame(Guid gameId);
        MakeGuessDto MakeGuess(Guid gameId, string letter);
        bool DeleteGame(Guid gameId);
    }
}