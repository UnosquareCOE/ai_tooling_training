using services.Dtos;

namespace services.Interfaces;

public interface IGameService
{
    Task<CreateGameResponseDto> CreateGame(CreateGameRequestDto request);
    GameDto? GetGame(Guid gameId);
    MakeGuessResponseDto? MakeGuess(Guid gameId, GuessDto guessDto);
    string? Cheat(Guid gameId);
    bool DeleteGame(Guid gameId);
}