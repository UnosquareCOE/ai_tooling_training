using AutoMapper;
using Game.Services.Dto;
using Game.Services.Interfaces;
using Game.Services.Services;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Game.Services.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IIdentifierGenerator> _mockIdentifierGenerator;
        private readonly IMapper _mapper;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _mockIdentifierGenerator = new Mock<IIdentifierGenerator>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GameDto, GameDto>().ReverseMap();
                cfg.CreateMap<MakeGuessDto, MakeGuessDto>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _gameService = new GameService(_mockIdentifierGenerator.Object, _mapper);
        }

        [Fact]
        public async Task CreateGame_ShouldReturnNewGameId()
        {
            // Arrange
            var newGameId = Guid.NewGuid();
            _mockIdentifierGenerator.Setup(generator => generator.RetrieveIdentifier()).Returns(newGameId);

            // Act
            var result = await _gameService.CreateGameAsync("en");

            // Assert
            Assert.Equal(newGameId, result);
        }

        [Fact]
        public async Task GetGame_ShouldReturnGameDto()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _mockIdentifierGenerator.Setup(generator => generator.RetrieveIdentifier()).Returns(gameId);
            await _gameService.CreateGameAsync("en");
            // Act
            var result = await _gameService.GetGameAsync(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.GameId);
        }

        [Fact]
        public async Task MakeGuess_ShouldReturnUpdatedMakeGuessDto()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _mockIdentifierGenerator.Setup(generator => generator.RetrieveIdentifier()).Returns(gameId);
            await _gameService.CreateGameAsync("en");

            // Act
            var result = _gameService.MakeGuess(gameId, "a");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("a", result.Guesses);
        }

        [Fact]
        public async Task DeleteGame_ShouldRemoveGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _mockIdentifierGenerator.Setup(generator => generator.RetrieveIdentifier()).Returns(gameId);
            await _gameService.CreateGameAsync("en");

            // Act
            _gameService.DeleteGame(gameId);
            var result = await _gameService.GetGameAsync(gameId);

            // Assert
            Assert.Null(result);
        }
    }
}