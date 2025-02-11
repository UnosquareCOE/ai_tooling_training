using AutoMapper;
using NSubstitute;
using services.Constants;
using services.Dtos;
using services.Interfaces;
using services.Profiles;
using services.Services;

namespace services.test.Services;

public class GameServiceTests
{
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public GameServiceTests()
    {
        _mapper = GetMapper();
        _httpClient = Substitute.For<HttpClient>();
        _httpClient.BaseAddress = new Uri("https://random-word-api.herokuapp.com");
    }
        
    private IGameService RetrieveService()
    {
        return new GameService(_mapper);
    }
    
    private static IMapper GetMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GameProfile>();
        });

        return new Mapper(config);
    }

    [Fact]
    public async Task CreateGame_ShouldReturnNewGame()
    {
        var request = new CreateGameRequestDto { Language = "en" };
        var gameService = RetrieveService();
        var response = await gameService.CreateGame(request);

        Assert.NotNull(response);
        Assert.NotEqual(Guid.Empty, response.GameId);
        Assert.Equal(5, response.AttemptsRemaining);
        Assert.NotNull(response.MaskedWord);
    }

    [Fact]
    public void GetGame_ShouldReturnGame_WhenGameExists()
    {
        var newId = Guid.NewGuid();
        var gameDto = new GameDto
        {
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = GameStatuses.InProgress
        };

        GameService.Games.Add(newId, gameDto);

        var gameService = RetrieveService();
        var result = gameService.GetGame(newId);

        Assert.NotNull(result);
        Assert.Equal(gameDto, result);
    }

    [Fact]
    public void GetGame_ShouldReturnNull_WhenGameDoesNotExist()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        var result = gameService.GetGame(newId);

        Assert.Null(result);
    }

    [Fact]
    public void MakeGuess_ShouldUpdateGame_WhenGuessIsCorrect()
    {
        var newId = Guid.NewGuid();
        var gameDto = new GameDto
        {
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = GameStatuses.InProgress
        };

        GameService.Games.Add(newId, gameDto);

        var gameService = RetrieveService();
        var guessDto = new GuessDto { Letter = "e" };
        var response = gameService.MakeGuess(newId, guessDto);

        Assert.NotNull(response);
        Assert.Equal("e_____e", response.MaskedWord);
        Assert.Equal(5, response.AttemptsRemaining);
        Assert.Empty(response.Guesses);
        Assert.Equal(GameStatuses.InProgress.ToString(), response.Status);
    }

    [Fact]
    public void MakeGuess_ShouldUpdateGame_WhenGuessIsIncorrect()
    {
        var newId = Guid.NewGuid();
        var gameDto = new GameDto
        {
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = new List<string>(),
            Status = GameStatuses.InProgress
        };

        GameService.Games.Add(newId, gameDto);

        var gameService = RetrieveService();
        var guessDto = new GuessDto { Letter = "z" };
        var response = gameService.MakeGuess(newId, guessDto);

        Assert.NotNull(response);
        Assert.Equal("_______", response.MaskedWord);
        Assert.Equal(4, response.AttemptsRemaining);
        Assert.Single(response.Guesses);
        Assert.Equal("z", response.Guesses[0]);
        Assert.Equal(GameStatuses.InProgress.ToString(), response.Status);
    }

    [Fact]
    public void Cheat_ShouldReturnUnmaskedWord_WhenGameExists()
    {
        var newId = Guid.NewGuid();

        var gameDto = new GameDto
        {
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = GameStatuses.InProgress
        };

        GameService.Games.Add(newId, gameDto);

        var gameService = RetrieveService();
        var result = gameService.Cheat(newId);

        Assert.Equal(7, result.Length);
    }

    [Fact]
    public void Cheat_ShouldReturnNull_WhenGameDoesNotExist()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        var result = gameService.Cheat(newId);

        Assert.Null(result);
    }

    [Fact]
    public void DeleteGame_ShouldReturnTrue_WhenGameExists()
    {
        var newId = Guid.NewGuid();
        var gameDto = new GameDto
        {
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = GameStatuses.InProgress
        };

        GameService.Games.Add(newId, gameDto);

        var gameService = RetrieveService();
        var result = gameService.DeleteGame(newId);

        Assert.True(result);
        Assert.Null(gameService.GetGame(newId));
    }

    [Fact]
    public void DeleteGame_ShouldReturnFalse_WhenGameDoesNotExist()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        var result = gameService.DeleteGame(newId);

        Assert.False(result);
    }
}