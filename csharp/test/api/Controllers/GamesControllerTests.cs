using api.Controllers;
using api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using NSubstitute;
using services.Constants;
using services.Dtos;
using services.Interfaces;

namespace api.test.Controllers;

public class GamesControllerTests
{
    private readonly IMapper _mapper;
    private readonly IGameService _service;

    public GamesControllerTests()
    {
        _mapper = Substitute.For<IMapper>();
        _service = Substitute.For<IGameService>();
    }

    private GamesController RetrieveController()
    {
        return new GamesController(_mapper, _service);
    }

    [Fact]
    public async Task CreateGame_WhenCalled_ReturnsValidIdentifier()
    {
        var newId = Guid.NewGuid();
        var request = new CreateGameRequestViewModel { Language = "en" };
        var createGameResponseDto = new CreateGameResponseDto
        {
            GameId = newId,
            MaskedWord = "_______",
            AttemptsRemaining = 5
        };

        _service.CreateGame(Arg.Any<CreateGameRequestDto>()).Returns(createGameResponseDto);
        _mapper.Map<CreateGameRequestDto>(request).Returns(new CreateGameRequestDto { Language = "en" });
        _mapper.Map<CreateGameResponseViewModel>(createGameResponseDto).Returns(new CreateGameResponseViewModel
        {
            GameId = newId,
            MaskedWord = "_______",
            AttemptsRemaining = 5
        });

        var gamesController = RetrieveController();
        var response = await gamesController.CreateGame(request);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<CreateGameResponseViewModel>(result.Value);
        Assert.Equal(newId, responseValue.GameId);
    }

    [Fact]
    public async Task GetGame_WhenGameExists_ReturnsGame()
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

        _service.GetGame(newId).Returns(gameDto);
        _mapper.Map<MakeGuessResponseViewModel>(gameDto).Returns(new MakeGuessResponseViewModel
        {
            MaskedWord = "_______",
            AttemptsRemaining = 5,
            Guesses = [],
            Status = GameStatuses.InProgress
        });

        var gamesController = RetrieveController();
        var response = await gamesController.GetGame(newId);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<MakeGuessResponseViewModel>(result.Value);
        Assert.Equal("_______", responseValue.MaskedWord);
        Assert.Equal(5, responseValue.AttemptsRemaining);
        Assert.Empty(responseValue.Guesses);
        Assert.Equal(GameStatuses.InProgress, responseValue.Status);
    }

    [Fact]
    public async Task MakeGuess_WhenGameExistsAndValidGuess_ReturnsUpdatedGame()
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
        var guessDto = new GuessDto { Letter = "e" };
        var makeGuessResponseDto = new MakeGuessResponseDto
        {
            MaskedWord = "e______",
            AttemptsRemaining = 5,
            Guesses = [],
            Status = GameStatuses.InProgress
        };

        _service.GetGame(newId).Returns(gameDto);
        _service.MakeGuess(newId, guessDto).Returns(makeGuessResponseDto);
        _mapper.Map<GuessDto>(Arg.Any<GuessViewModel>()).Returns(guessDto);
        _mapper.Map<MakeGuessResponseViewModel>(makeGuessResponseDto).Returns(new MakeGuessResponseViewModel
        {
            MaskedWord = "e______",
            AttemptsRemaining = 5,
            Guesses = [],
            Status = GameStatuses.InProgress
        });

        var gamesController = RetrieveController();
        var guess = new GuessViewModel { Letter = "e" };
        var response = await gamesController.MakeGuess(newId, guess);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<MakeGuessResponseViewModel>(result.Value);
        Assert.Equal("e______", responseValue.MaskedWord);
        Assert.Equal(5, responseValue.AttemptsRemaining);
        Assert.Empty(responseValue.Guesses);
        Assert.Equal(GameStatuses.InProgress, responseValue.Status);
    }

    [Fact]
    public async Task MakeGuess_WhenGameExistsAndInvalidGuess_ReturnsUpdatedGame()
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
        var guessDto = new GuessDto { Letter = "z" };
        var makeGuessResponseDto = new MakeGuessResponseDto
        {
            MaskedWord = "_______",
            AttemptsRemaining = 4,
            Guesses = ["z"],
            Status = GameStatuses.InProgress
        };

        _service.GetGame(newId).Returns(gameDto);
        _service.MakeGuess(newId, guessDto).Returns(makeGuessResponseDto);
        _mapper.Map<GuessDto>(Arg.Any<GuessViewModel>()).Returns(guessDto);
        _mapper.Map<MakeGuessResponseViewModel>(makeGuessResponseDto).Returns(new MakeGuessResponseViewModel
        {
            MaskedWord = "_______",
            AttemptsRemaining = 4,
            Guesses = ["z"],
            Status = GameStatuses.InProgress
        });

        var gamesController = RetrieveController();
        var guess = new GuessViewModel { Letter = "z" };
        var response = await gamesController.MakeGuess(newId, guess);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<MakeGuessResponseViewModel>(result.Value);
        Assert.Equal("_______", responseValue.MaskedWord);
        Assert.Equal(4, responseValue.AttemptsRemaining);
        Assert.Single(responseValue.Guesses);
        Assert.Equal("z", responseValue.Guesses[0]);
        Assert.Equal(GameStatuses.InProgress, responseValue.Status);
    }

    [Fact]
    public async Task DeleteGame_WhenGameExists_ReturnsNoContent()
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
    
        _service.GetGame(newId).Returns(gameDto);
        _service.DeleteGame(newId).Returns(true);
    
        var gamesController = RetrieveController();
        var response = await gamesController.DeleteGame(newId);
    
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task DeleteGame_WhenGameDoesNotExist_ReturnsNotFound()
    {
        var newId = Guid.NewGuid();
        _service.DeleteGame(newId).Returns(false);

        var gamesController = RetrieveController();
        var response = await gamesController.DeleteGame(newId);

        var result = Assert.IsType<NotFoundObjectResult>(response);
        var responseValue = Assert.IsType<ResponseErrorViewModel>(result.Value);
        Assert.Equal("Game not found", responseValue.Message);
        Assert.Single(responseValue.Errors);
        Assert.Equal("gameId", responseValue.Errors[0].Field);
        Assert.Equal("The specified game ID does not exist.", responseValue.Errors[0].Message);
    }
}