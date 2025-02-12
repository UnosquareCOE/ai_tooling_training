using System.Text.RegularExpressions;
using AutoMapper;
using dal.Interfaces;
using dal.Models;
using services.Constants;
using services.Dtos;
using services.Interfaces;

namespace services.Services;

public partial class GameService : IGameService
{
    private readonly IMapper _mapper;
    private readonly IGameContext _gameContext;

    public GameService(IMapper mapper, IGameContext gameContext)
    {
        _mapper = mapper;
        _gameContext = gameContext;
    }

    public async Task<CreateGameResponseDto> CreateGame(CreateGameRequestDto request)
    {
        var newGameId = Guid.NewGuid();
        var newGameWord = await RetrieveWord(request.Language);
        var newGame = new Game
        {
            Id = newGameId,
            RemainingGuesses = 5,
            UnmaskedWord = newGameWord,
            Word = GuessRegex().Replace(newGameWord, "_"),
            Status = GameStatuses.InProgress,
            IncorrectGuesses = []
        };
        _gameContext.Games.Add(newGame);
        var response = _mapper.Map<CreateGameResponseDto>(newGame);
        response.GameId = newGameId;
        await _gameContext.SaveChangesAsync();
        return response;
    }

    public async Task<GameDto?> GetGame(Guid gameId)
    {
        var game = await _gameContext.Games.FindAsync(gameId);
        return game == null ? null : _mapper.Map<GameDto>(game);
    }

    public async Task<MakeGuessResponseDto?> MakeGuess(Guid gameId, GuessDto guessViewModel)
    {
        var game = await _gameContext.Games.FindAsync(gameId);
        if (game == null) return null;
        
        var guessedLetter = guessViewModel.Letter!.ToLower();
        ProcessGuess(game, guessedLetter);
        await _gameContext.SaveChangesAsync();
        return _mapper.Map<MakeGuessResponseDto>(game);
    }

    public async Task<string?> Cheat(Guid gameId)
    {
        var game = await _gameContext.Games.FindAsync(gameId);
        return game?.UnmaskedWord;
    }

    public async Task<bool> DeleteGame(Guid gameId)
    {
        var game = await _gameContext.Games.FindAsync(gameId);
        if (game == null)
        {
            return false;
        }
        _gameContext.Games.Remove(game);
        await _gameContext.SaveChangesAsync();
        return true;
    }

    private static async Task<string> RetrieveWord(string language)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync($"https://random-word-api.herokuapp.com/word?lang={language}&length=7");
        var words = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response);
        return words?.FirstOrDefault() ?? "default";
    }

    private static void ProcessGuess(Game game, string guessedLetter)
    {
        var unmaskedWord = game.UnmaskedWord!.ToLower();
        var maskedWord = game.Word!.ToCharArray();
        var correctGuess = false;

        for (var i = 0; i < unmaskedWord.Length; i++)
        {
            if (unmaskedWord[i] != guessedLetter[0]) continue;
            maskedWord[i] = unmaskedWord[i];
            correctGuess = true;
        }

        game.Word = new string(maskedWord);

        if (!correctGuess)
        {
            game.IncorrectGuesses.Add(guessedLetter);
            game.RemainingGuesses--;
        }

        if (game.Word == game.UnmaskedWord)
        {
            game.Status = GameStatuses.GameWon;
        }
        else if (game.RemainingGuesses == 0)
        {
            game.Status = GameStatuses.GameLost;
        }
    }
    
    private static Regex GuessRegex()
    {
        return MyRegex();
    }
    
    [GeneratedRegex(@"[a-zA-Z0-9_]")]
    private static partial Regex MyRegex();
}