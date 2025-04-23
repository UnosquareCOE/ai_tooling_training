using api.Controllers;
using api.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using service.dtos;
using service.interfaces;

namespace api.Tests.Controllers
{
    public class GamesControllerTest
    {
        private readonly IHangmanGameService _gameService;
        private readonly IMapper _mapper;
        private readonly GamesController _controller;

        public GamesControllerTest()
        {
            _gameService = Substitute.For<IHangmanGameService>();
            _mapper = Substitute.For<IMapper>();
            _controller = new GamesController(_gameService, _mapper);
        }

        [Fact]
        public void CreateGame_ReturnsOkResult_WithCreateGameViewModel()
        {
            // Arrange
            var gameDto = new GameDto(); // Assuming GameDto is the DTO returned by the service
            var createGameViewModel = new CreateGameViewModel(); // Assuming CreateGameViewModel is the ViewModel

            _gameService.CreateGame().Returns(gameDto);
            _mapper.Map<CreateGameViewModel>(gameDto).Returns(createGameViewModel);

            // Act
            var result = _controller.CreateGame();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CreateGameViewModel>(okResult.Value);
            Assert.Equal(createGameViewModel, returnValue);
        }

        [Fact]
        public void GetGame_InvalidGuid_ReturnsBadRequest()
        {
            // Arrange
            var invalidGuid = Guid.Empty;
            var errorResponse = new ResponseErrorViewModel { Message = "Invalid GUID" };

            // Act
            var result = _controller.GetGame(invalidGuid);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(errorResponse.ToString(), badRequestResult.Value.ToString());
        }

        [Fact]
        public void GetGame_GameNotFound_ReturnsNotFound()
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            _gameService.GetGameById(validGuid).Returns((GameDto)null);

            // Act
            var result = _controller.GetGame(validGuid);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var errorResponse = Assert.IsType<ResponseErrorViewModel>(notFoundResult.Value);
            Assert.Equal("Game not found", errorResponse.Message);
        }

        [Fact]
        public void GetGame_GameFound_ReturnsOk()
        {
            // Arrange
            var validGuid = Guid.NewGuid();
            var gameDto = new GameDto();
            var checkGameViewModel = new CheckGameViewModel();

            _gameService.GetGameById(validGuid).Returns(gameDto);
            _mapper.Map<CheckGameViewModel>(gameDto).Returns(checkGameViewModel);

            // Act
            var result = _controller.GetGame(validGuid);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CheckGameViewModel>(okResult.Value);
            Assert.Equal(checkGameViewModel, returnValue);
        }

        [Fact]
        public void ClearGame_ReturnsBadRequest_WhenGuidIsInvalid()
        {
            // Arrange
            var invalidGuid = Guid.Empty;

            // Act
            var result = _controller.ClearGame(invalidGuid);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void ClearGame_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _gameService.ClearGame(gameId).Returns((Guid?)null);

            // Act
            var result = _controller.ClearGame(gameId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ClearGame_ReturnsNoContent_WhenGameIsCleared()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _gameService.ClearGame(gameId).Returns(gameId);

            // Act
            var result = _controller.ClearGame(gameId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void MakeGuess_ReturnsBadRequest_WhenGuessIsInvalid()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var guessViewModel = new GuessViewModel { Letter = null };

            // Act
            var result = _controller.MakeGuess(gameId, guessViewModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void MakeGuess_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var guessViewModel = new GuessViewModel { Letter = 'a' };
            _gameService.MakeGuess(gameId, 'a').Returns((GameDto)null);

            // Act
            var result = _controller.MakeGuess(gameId, guessViewModel);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void MakeGuess_ReturnsOkResult_WithCheckGameViewModel()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var guessViewModel = new GuessViewModel { Letter = 'a' };
            var gameDto = new GameDto();
            var viewModel = new CheckGameViewModel();
            _gameService.MakeGuess(gameId, 'a').Returns(gameDto);
            _mapper.Map<CheckGameViewModel>(gameDto).Returns(viewModel);

            // Act
            var result = _controller.MakeGuess(gameId, guessViewModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<CheckGameViewModel>(okResult.Value);
        }

    }
}