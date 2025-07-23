using api.Controllers;
using api.Utils;
using api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace api.test.Controllers;

public class MockIdentifierGenerator(Guid newId) : IIdentifierGenerator
{
    public Guid RetrieveIdentifier()
    {
        return newId;
    }
}

public class GamesControllerTests
{
    [Fact]
    public void CreateGame_WhenCalled_ReturnsValidIdentifier()
    {
        var newId = Guid.NewGuid();

        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var response = gamesController.CreateGame();

        Assert.IsType<OkObjectResult>(response.Result);
        Assert.Equal(newId, ((OkObjectResult)response.Result).Value);
    }

    [Fact]
    public void DeleteGame_WithInProgressGame_ReturnsNoContent()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game first
        gamesController.CreateGame();
        
        // Act
        var deleteResponse = gamesController.DeleteGame(gameId);
        
        // Assert
        Assert.IsType<NoContentResult>(deleteResponse);
        
        // Verify game is deleted
        var getResponse = gamesController.GetGame(gameId);
        Assert.IsType<OkObjectResult>(getResponse.Result);
        var game = ((OkObjectResult)getResponse.Result).Value as GameViewModel;
        Assert.Null(game);
    }

    [Fact]
    public void DeleteGame_WithNonExistentGame_ReturnsNotFound()
    {
        // Arrange
        var gamesController = RetrieveController(new MockIdentifierGenerator(Guid.NewGuid()));
        var nonExistentId = Guid.NewGuid();
        
        // Act
        var deleteResponse = gamesController.DeleteGame(nonExistentId);
        
        // Assert
        Assert.IsType<NotFoundResult>(deleteResponse);
    }

    [Fact]
    public void DeleteGame_WithWonGame_ReturnsBadRequest()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game and simulate it being won
        gamesController.CreateGame();
        
        // We need to access the game and change its status
        // Since the Games dictionary is private static, we'll use reflection
        var gamesField = typeof(GamesController).GetField("Games", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var games = gamesField?.GetValue(null) as Dictionary<Guid, GameViewModel>;
        
        if (games != null && games.ContainsKey(gameId))
        {
            games[gameId].Status = "Won";
        }
        
        // Act
        var deleteResponse = gamesController.DeleteGame(gameId);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(deleteResponse);
        var errorResponse = ((BadRequestObjectResult)deleteResponse).Value as ResponseErrorViewModel;
        Assert.NotNull(errorResponse);
        Assert.Equal("Only games with status 'In Progress' can be deleted", errorResponse.Message);
    }

    [Fact]
    public void DeleteGame_WithLostGame_ReturnsBadRequest()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game and simulate it being lost
        gamesController.CreateGame();
        
        // We need to access the game and change its status
        var gamesField = typeof(GamesController).GetField("Games", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var games = gamesField?.GetValue(null) as Dictionary<Guid, GameViewModel>;
        
        if (games != null && games.ContainsKey(gameId))
        {
            games[gameId].Status = "Lost";
        }
        
        // Act
        var deleteResponse = gamesController.DeleteGame(gameId);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(deleteResponse);
        var errorResponse = ((BadRequestObjectResult)deleteResponse).Value as ResponseErrorViewModel;
        Assert.NotNull(errorResponse);
        Assert.Equal("Only games with status 'In Progress' can be deleted", errorResponse.Message);
    }

    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}