using api.Controllers;
using api.ViewModels;
using AutoMapper;
using Game.Services.Dto;
using Game.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace api.test.Controllers;

public class GamesControllerTests
{
    private readonly Mock<IGameService> _mockGameService;
    private readonly IMapper _mapper;
    private readonly GamesController _gamesController;

    public GamesControllerTests()
    {
        _mockGameService = new Mock<IGameService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GameDto, GameViewModel>().ReverseMap();
            cfg.CreateMap<MakeGuessDto, MakeGuessViewModel>().ReverseMap();
            cfg.CreateMap<GameDto, CreateGameViewModel>() 
                .ForMember(d => d.MaskedWord, o => o.MapFrom(x => x.Word))
                .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(x => x.RemainingGuesses));

            cfg.CreateMap<GameDto, CheckGameStatusViewModel>().ForMember(d => d.MaskedWord, o => o.MapFrom(x => x.Word))
                .ForMember(d => d.AttemptsRemaining, o => o.MapFrom(x => x.RemainingGuesses))
                .ForMember(d => d.Guesses, o => o.MapFrom(x => x.IncorrectGuesses));

        });
        _mapper = config.CreateMapper();

        _gamesController = new GamesController(_mockGameService.Object, _mapper);
    }

    [Fact]
    public void CreateGame_WhenCalled_ReturnsCreatedAtActionResult()
    {
        var newGameId = Guid.NewGuid();
        var gameDto = new GameDto
        {
            GameId = newGameId,
            Word = "______",
            RemainingGuesses = 5,
            Status = "In Progress",
            IncorrectGuesses = new List<string>()
        };

        _mockGameService.Setup(service => service.CreateGame()).Returns(newGameId);
        _mockGameService.Setup(service => service.GetGame(newGameId)).Returns(gameDto);

        var response = _gamesController.CreateGame();

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var model = Assert.IsType<CreateGameViewModel>(result.Value);
        Assert.Equal(newGameId, model.GameId);
        Assert.Equal(5, model.AttemptsRemaining);
    }

    [Fact]
    public void GetGame_WhenCalled_ReturnsOkObjectResult()
    {
        var gameId = Guid.NewGuid();
        var gameDto = new GameDto
        {
            GameId = gameId,
            Word = "______",
            UnmaskedWord = "banana",
            RemainingGuesses = 5,
            Status = "In Progress",
            IncorrectGuesses = new List<string>()
        };

        _mockGameService.Setup(service => service.GetGame(gameId)).Returns(gameDto);

        var response = _gamesController.GetGame(gameId);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var model = Assert.IsType<CheckGameStatusViewModel>(result.Value);
        Assert.Equal(5, model.AttemptsRemaining);
    }

    [Fact]
    public void MakeGuess_WhenCalledWithValidGuess_ReturnsOkObjectResult()
    {
        var gameId = Guid.NewGuid();
        var makeGuessDto = new MakeGuessDto
        {
            MaskedWord = "b_____",
            AttemptsRemaining = 4,
            Guesses = new List<string> { "b" },
            Status = "In Progress"
        };

        _mockGameService.Setup(service => service.MakeGuess(gameId, "b")).Returns(makeGuessDto);

        var guessViewModel = new GuessViewModel { Letter = "b" };
        var response = _gamesController.MakeGuess(gameId, guessViewModel);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var model = Assert.IsType<MakeGuessViewModel>(result.Value);
        Assert.Contains("b", model.Guesses);
    }

    [Fact]
    public void MakeGuess_WhenCalledWithInvalidGuess_ReturnsBadRequest()
    {
        var gameId = Guid.NewGuid();
        var guessViewModel = new GuessViewModel { Letter = "bb" };

        var response = _gamesController.MakeGuess(gameId, guessViewModel);

        var result = Assert.IsType<BadRequestObjectResult>(response.Result);
        var value = Assert.IsType<ResponseErrorViewModel>(result.Value);

        Assert.Equal("Cannot process guess", value.Message);
        Assert.Single(value.Errors);
        Assert.Equal("letter", value.Errors[0].Field);
        Assert.Equal("Letter cannot accept more than 1 character", value.Errors[0].Message);
    }

    [Fact]
    public void DeleteGame_WhenCalled_ReturnsNoContent()
    {
        var gameId = Guid.NewGuid();

        _mockGameService.Setup(service => service.DeleteGame(gameId));

        var response = _gamesController.DeleteGame(gameId);

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public void DeleteGame_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var gameId = Guid.NewGuid();

        _mockGameService.Setup(service => service.DeleteGame(gameId)).Throws(new KeyNotFoundException());

        var response = _gamesController.DeleteGame(gameId);

        Assert.IsType<NotFoundObjectResult>(response);
    }
}