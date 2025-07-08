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
    public void DeleteGame_WhenGameExistsAndInProgress_ReturnsNoContent()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        gamesController.CreateGame(); // Create a game first
        
        // Act
        var result = gamesController.DeleteGame(gameId);
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteGame_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Act
        var result = gamesController.DeleteGame(gameId);
        
        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = (NotFoundObjectResult)result;
        Assert.NotNull(notFoundResult.Value);
        var errorResponse = (ResponseErrorViewModel)notFoundResult.Value;
        Assert.Equal("Game not found", errorResponse.Message);
    }

    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}