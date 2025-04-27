using AutoMapper;
using Game.DAL.Interfaces;
using Game.Services.Dto;
using Game.Services.Services;
using NSubstitute;
using Xunit;
using Game.Services.Interfaces;
using Game.Services.Profiles;
using Assert = Xunit.Assert;

namespace Game.Services.Tests.Services
{
    public class GameServiceTests
    {
        private readonly IIdentifierGenerator _mockIdentifierGenerator;
        private readonly IGameContext _mockGameContext;
        private readonly IMapper _mapper;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _mockIdentifierGenerator = Substitute.For<IIdentifierGenerator>();
            _mockGameContext = Substitute.For<IGameContext>();
            _mapper = GetMapper();
            _gameService = new GameService(_mockIdentifierGenerator, _mapper, _mockGameContext);
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
        public async Task CreateGame_ShouldReturnNewGameId()
        {
            // Arrange
            var newGameId = Guid.NewGuid();
            _mockIdentifierGenerator.RetrieveIdentifier().Returns(newGameId);
            _mockGameContext.SaveChangesAsync().Returns(Task.FromResult(1));

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
            var game = new DAL.Models.Game
            {
                Id = gameId,
                UnmaskedWord = "banana",
                Word = "______",
                RemainingGuesses = 5,
                Status = "In Progress",
                IncorrectGuesses = new List<string>()
            };
            _mockGameContext.Games.FindAsync(gameId).Returns(game);

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
            var game = new DAL.Models.Game
            {
                Id = gameId,
                UnmaskedWord = "banana",
                Word = "______",
                RemainingGuesses = 5,
                Status = "In Progress",
                IncorrectGuesses = new List<string>()
            };
            _mockGameContext.Games.FindAsync(gameId).Returns(game);
            _mockGameContext.SaveChangesAsync().Returns(Task.FromResult(1));

            // Act
            var result = await _gameService.MakeGuess(gameId, "a");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("a", result.Guesses);
        }

        [Fact]
        public async Task DeleteGame_ShouldRemoveGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = new DAL.Models.Game
            {
                Id = gameId,
                UnmaskedWord = "banana",
                Word = "______",
                RemainingGuesses = 5,
                Status = "In Progress",
                IncorrectGuesses = new List<string>()
            };
            _mockGameContext.Games.FindAsync(gameId).Returns(game);
            _mockGameContext.SaveChangesAsync().Returns(Task.FromResult(1));

            // Act
            var result = await _gameService.DeleteGame(gameId);

            // Assert
            Assert.True(result);
        }
    }
}