using api.Controllers;
using api.Utils;
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
    public void DeleteGame_WhenGameExists_AndStatusIsInProgress_ReturnsNoContent()
    {
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game first
        gamesController.CreateGame();
        
        // Delete the game
        var response = gamesController.DeleteGame(gameId);
        
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public void DeleteGame_WhenGameDoesNotExist_ReturnsNotFound()
    {
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Try to delete a game that doesn't exist
        var response = gamesController.DeleteGame(gameId);
        
        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public void DeleteGame_WhenGameStatusIsWon_ReturnsBadRequest()
    {
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game first
        gamesController.CreateGame();
        
        // Get the game and manually set its status to "Won" for testing
        var game = gamesController.GetGame(gameId);
        var gameResult = (OkObjectResult)game.Result!;
        var gameViewModel = (api.ViewModels.GameViewModel)gameResult.Value!;
        gameViewModel.Status = "Won";
        
        // Try to delete the game
        var response = gamesController.DeleteGame(gameId);
        
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public void DeleteGame_WhenGameStatusIsLost_ReturnsBadRequest()
    {
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game first
        gamesController.CreateGame();
        
        // Get the game and manually set its status to "Lost" for testing
        var game = gamesController.GetGame(gameId);
        var gameResult = (OkObjectResult)game.Result!;
        var gameViewModel = (api.ViewModels.GameViewModel)gameResult.Value!;
        gameViewModel.Status = "Lost";
        
        // Try to delete the game
        var response = gamesController.DeleteGame(gameId);
        
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public void DeleteGame_AfterDeletion_GameIsRemovedFromCollection()
    {
        var gameId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(gameId));
        
        // Create a game first
        gamesController.CreateGame();
        
        // Verify game exists
        var gameBeforeDelete = gamesController.GetGame(gameId);
        Assert.IsType<OkObjectResult>(gameBeforeDelete.Result);
        
        // Delete the game
        gamesController.DeleteGame(gameId);
        
        // Verify game no longer exists
        var gameAfterDelete = gamesController.GetGame(gameId);
        Assert.IsType<OkObjectResult>(gameAfterDelete.Result);
        Assert.Null(((OkObjectResult)gameAfterDelete.Result).Value);
    }

    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}