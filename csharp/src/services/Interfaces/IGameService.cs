using services.Dtos;

namespace services.Interfaces;

public interface IGameService
{
    Task<CreateGameResponseDto> CreateGame(CreateGameRequestDto request);
    Task<GameDto?> GetGame(Guid gameId);
    Task<MakeGuessResponseDto?> MakeGuess(Guid gameId, GuessDto guessDto);
    Task<string?> Cheat(Guid gameId);
    Task<bool> DeleteGame(Guid gameId);
}