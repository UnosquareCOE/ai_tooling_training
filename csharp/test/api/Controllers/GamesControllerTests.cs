using api.Controllers;
using api.Profiles;
using api.RequestModels;
using api.ViewModels;
using AutoMapper;
using Game.Services.Dto;
using Game.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api.test.Controllers
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly IMapper _mapper;
        private readonly GamesController _gamesController;

        public GamesControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _mapper = GetMapper();
            _gamesController = new GamesController(_mockGameService.Object, _mapper);
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
        public async Task CreateGame_WhenCalled_ReturnsOkObjectResult()
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

            _mockGameService.Setup(service => service.CreateGameAsync(It.IsAny<string>())).ReturnsAsync(newGameId);
            _mockGameService.Setup(service => service.GetGameAsync(newGameId)).ReturnsAsync(gameDto);

            var requestModel = new CreateGameRequestModel { Language = "en" };
            var response = await _gamesController.CreateGame(requestModel);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            var model = Assert.IsType<CreateGameViewModel>(result.Value);
            Assert.Equal(newGameId, model.GameId);
            Assert.Equal(5, model.AttemptsRemaining);
        }

        [Fact]
        public async Task GetGame_WhenCalled_ReturnsOkObjectResult()
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

            _mockGameService.Setup(service => service.GetGameAsync(gameId)).ReturnsAsync(gameDto);

            var response = await _gamesController.GetGame(gameId);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            var model = Assert.IsType<CheckGameStatusViewModel>(result.Value);
            Assert.Equal(5, model.AttemptsRemaining);
        }

        [Fact]
        public async Task MakeGuess_WhenCalledWithValidGuess_ReturnsOkObjectResult()
        {
            var gameId = Guid.NewGuid();
            var makeGuessDto = new MakeGuessDto
            {
                MaskedWord = "b_____",
                AttemptsRemaining = 4,
                Guesses = new List<string> { "b" },
                Status = "In Progress"
            };

            _mockGameService.Setup(service => service.MakeGuess(gameId, "b")).ReturnsAsync(makeGuessDto);

            var guessViewModel = new GuessViewModel { Letter = "b" };
            var response = await _gamesController.MakeGuess(gameId, guessViewModel);

            var result = Assert.IsType<OkObjectResult>(response.Result);
            var model = Assert.IsType<MakeGuessViewModel>(result.Value);
            Assert.Contains("b", model.Guesses);
        }

        [Fact]
        public async Task MakeGuess_WhenCalledWithInvalidGuess_ReturnsBadRequest()
        {
            var gameId = Guid.NewGuid();
            var guessViewModel = new GuessViewModel { Letter = "bb" };

            var response = await _gamesController.MakeGuess(gameId, guessViewModel);

            var result = Assert.IsType<BadRequestObjectResult>(response.Result);
            var value = Assert.IsType<ResponseErrorViewModel>(result.Value);

            Assert.Equal("Cannot process guess", value.Message);
            Assert.Single(value.Errors);
            Assert.Equal("letter", value.Errors[0].Field);
            Assert.Equal("Letter cannot accept more than 1 character", value.Errors[0].Message);
        }

        [Fact]
        public async Task DeleteGame_WhenCalled_ReturnsNoContent()
        {
            var gameId = Guid.NewGuid();

            _mockGameService.Setup(service => service.DeleteGame(gameId)).ReturnsAsync(true);

            var response = await _gamesController.DeleteGame(gameId);

            Assert.IsType<NotFoundObjectResult>(response);
        }

        [Fact]
        public async Task DeleteGame_WhenCalledWithInvalidId_ReturnsNotFound()
        {
            var gameId = Guid.NewGuid();

            _mockGameService.Setup(service => service.DeleteGame(gameId)).ReturnsAsync(false);

            var response = await _gamesController.DeleteGame(gameId);

            var result = Assert.IsType<NotFoundObjectResult>(response);
            var value = Assert.IsType<ResponseErrorViewModel>(result.Value);

            Assert.Equal("Game not found", value.Message);
            Assert.Single(value.Errors);
            Assert.Equal("gameId", value.Errors[0].Field);
            Assert.Equal("The specified game ID does not exist.", value.Errors[0].Message);
        }
    }
}