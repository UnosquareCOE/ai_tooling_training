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
        var controller = RetrieveController(new MockIdentifierGenerator(gameId));
        controller.CreateGame();

        // Act
        var result = controller.DeleteGame(gameId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteGame_WhenGameNotFound_ReturnsNotFound()
    {
        // Arrange
        var controller = RetrieveController(new MockIdentifierGenerator(Guid.NewGuid()));
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = controller.DeleteGame(nonExistentId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = (NotFoundObjectResult)result;
        var error = notFoundResult.Value as ResponseErrorViewModel;
        Assert.NotNull(error);
        Assert.Equal("Game not found", error.Message);
    }

    [Fact]
    public void DeleteGame_WhenGameIsWon_ReturnsBadRequest()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var controller = RetrieveController(new MockIdentifierGenerator(gameId));
        controller.CreateGame();
        
        // Get the game and make correct guesses to win it
        var game = controller.GetGame(gameId);
        var gameViewModel = ((OkObjectResult)game.Result).Value as GameViewModel;
        
        // Make guesses for each letter in the word to win the game
        foreach (char c in gameViewModel!.UnmaskedWord!.ToLower().Distinct())
        {
            controller.MakeGuess(gameId, new GuessViewModel { Letter = c.ToString() });
        }

        // Act
        var result = controller.DeleteGame(gameId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = (BadRequestObjectResult)result;
        var error = badRequestResult.Value as ResponseErrorViewModel;
        Assert.NotNull(error);
        Assert.Equal("Cannot delete games with status Won or Lost", error.Message);
    }

    [Fact]
    public void DeleteGame_WhenGameIsLost_ReturnsBadRequest()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var controller = RetrieveController(new MockIdentifierGenerator(gameId));
        controller.CreateGame();
        
        // Make 3 incorrect guesses to lose the game
        controller.MakeGuess(gameId, new GuessViewModel { Letter = "z" });
        controller.MakeGuess(gameId, new GuessViewModel { Letter = "x" });
        controller.MakeGuess(gameId, new GuessViewModel { Letter = "q" });

        // Act
        var result = controller.DeleteGame(gameId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        var badRequestResult = (BadRequestObjectResult)result;
        var error = badRequestResult.Value as ResponseErrorViewModel;
        Assert.NotNull(error);
        Assert.Equal("Cannot delete games with status Won or Lost", error.Message);
    }

    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}