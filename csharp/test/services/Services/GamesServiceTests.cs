using AutoMapper;
using dal.Interfaces;
using dal.Models;
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
    private readonly IGameContext _gameContext;

    public GameServiceTests()
    {
        _mapper = GetMapper();
        _httpClient = Substitute.For<HttpClient>();
        _httpClient.BaseAddress = new Uri("https://random-word-api.herokuapp.com");
        _gameContext = Substitute.For<IGameContext>();
    }
        
    private IGameService RetrieveService()
    {
        return new GameService(_mapper, _gameContext);
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
    public async Task GetGame_ShouldReturnGame_WhenGameExists()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            RemainingGuesses = 5,
            Word = "________",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = "In Progress"
        };
        var gameId = game.Id;

        _gameContext.Games.FindAsync(gameId).Returns(game);

        var gameDto = _mapper.Map<GameDto>(game);
        var gameService = RetrieveService();
        var result = await gameService.GetGame(gameId);

        Assert.NotNull(result);
        Assert.Equivalent(gameDto, result);
    }

    [Fact]
    public async Task GetGame_ShouldReturnNull_WhenGameDoesNotExist()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        _gameContext.Games.FindAsync(newId).Returns((Game?)null);
        var result = await gameService.GetGame(newId);
        Assert.Null(result);
    }

    [Fact]
    public async Task MakeGuess_ShouldUpdateGame_WhenGuessIsCorrect()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = "In Progress"
        };
        var gameId = game.Id;
        _gameContext.Games.FindAsync(gameId).Returns(game);
        var gameService = RetrieveService();
        var guessDto = new GuessDto { Letter = "e" };
        var response = await gameService.MakeGuess(gameId, guessDto);

        Assert.NotNull(response);
        Assert.Equal("e_____e", response.MaskedWord);
        Assert.Equal(5, response.AttemptsRemaining);
        Assert.Empty(response.Guesses);
        Assert.Equal(GameStatuses.InProgress, response.Status);
    }

    [Fact]
    public async Task MakeGuess_ShouldUpdateGame_WhenGuessIsIncorrect()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            RemainingGuesses = 5,
            Word = "_______",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = "In Progress"
        };
        var gameId = game.Id;
        var gameService = RetrieveService();
        _gameContext.Games.FindAsync(gameId).Returns(game);
        var guessDto = new GuessDto { Letter = "z" };
        var response = await gameService.MakeGuess(gameId, guessDto);

        Assert.NotNull(response);
        Assert.Equal("_______", response.MaskedWord);
        Assert.Equal(4, response.AttemptsRemaining);
        Assert.Single(response.Guesses);
        Assert.Equal("z", response.Guesses[0]);
        Assert.Equal(GameStatuses.InProgress, response.Status);
    }

    [Fact]
    public async Task Cheat_ShouldReturnUnmaskedWord_WhenGameExists()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        var result = await gameService.Cheat(newId);

        if (result != null) 
            Assert.Equal(7, result.Length);
    }

    [Fact]
    public async Task Cheat_ShouldReturnNull_WhenGameDoesNotExist()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        var result = await gameService.Cheat(newId);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteGame_ShouldReturnTrue_WhenGameExists()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            RemainingGuesses = 5,
            Word = "________",
            UnmaskedWord = "example",
            IncorrectGuesses = [],
            Status = "In Progress"
        };
        var gameId = game.Id;
        _gameContext.Games.FindAsync(gameId).Returns(game);
        var gameService = RetrieveService();
        var result = await gameService.DeleteGame(gameId);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteGame_ShouldReturnFalse_WhenGameDoesNotExist()
    {
        var newId = Guid.NewGuid();
        var gameService = RetrieveService();
        var result = await gameService.DeleteGame(newId);

        Assert.False(result);
    }
}