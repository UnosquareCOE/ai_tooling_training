using System.Text.RegularExpressions;
using AutoMapper;
using services.Constants;
using services.Dtos;
using services.Interfaces;

namespace services.Services;

public partial class GameService : IGameService
{
    private static readonly Dictionary<Guid, GameDto> Games = new();
    private readonly IMapper _mapper;

    public GameService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<CreateGameResponseDto> CreateGame(CreateGameRequestDto request)
    {
        var newGameId = Guid.NewGuid();
        var newGameWord = await RetrieveWord(request.Language);

        var newGame = new GameDto
        {
            RemainingGuesses = 5,
            UnmaskedWord = newGameWord,
            Word = GuessRegex().Replace(newGameWord, "_"),
            Status = GameStatuses.InProgress,
            IncorrectGuesses = []
        };

        Games.Add(newGameId, newGame);

        return _mapper.Map<CreateGameResponseDto>(newGame);
    }

    public GameDto? GetGame(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    public MakeGuessResponseDto? MakeGuess(Guid gameId, GuessDto guessViewModel)
    {
        var game = GetGame(gameId);
        if (game == null) return null;

        var guessedLetter = guessViewModel.Letter!.ToLower();
        ProcessGuess(game, guessedLetter);

        return _mapper.Map<MakeGuessResponseDto>(game);
    }

    public string? Cheat(Guid gameId)
    {
        var game = GetGame(gameId);
        return game?.UnmaskedWord;
    }

    public bool DeleteGame(Guid gameId)
    {
        var game = GetGame(gameId);
        if (game == null)
        {
            return false;
        }

        Games.Remove(gameId);
        return true;
    }

    private static async Task<string> RetrieveWord(string language)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync($"https://random-word-api.herokuapp.com/word?lang={language}&length=7");
        var words = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response);
        return words?.FirstOrDefault() ?? "default";
    }

    private static void ProcessGuess(GameDto game, string guessedLetter)
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