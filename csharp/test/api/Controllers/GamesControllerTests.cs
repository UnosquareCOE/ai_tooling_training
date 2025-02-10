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
    public void CreateGame_WhenCalled_ReturnsCreatedAtActionResult()
    {
        var newId = Guid.NewGuid();

        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var response = gamesController.CreateGame();

        var result = Assert.IsType<CreatedAtActionResult>(response.Result);
        var model = Assert.IsType<CreateGameViewModel>(result.Value);
        Assert.Equal(newId, model.GameId);
        Assert.Equal("_______", model.MaskedWord);
        Assert.Equal(5, model.AttemptsRemaining);
    }

    [Fact]
    public void MakeGuess_WhenCalledWithValidGuess_ReturnsOkObjectResult()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        gamesController.CreateGame();

        var guessViewModel = new GuessViewModel { Letter = "a" };
        var response = gamesController.MakeGuess(newId, guessViewModel);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var model = Assert.IsType<MakeGuessViewModel>(result.Value);
        Assert.Contains("a", model.Guesses);
    }

    [Fact]
    public void MakeGuess_WhenCalledWithInvalidGuess_ReturnsBadRequest()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        gamesController.CreateGame();

        var guessViewModel = new GuessViewModel { Letter = "aa" };
        var response = gamesController.MakeGuess(newId, guessViewModel);

        var result = Assert.IsType<BadRequestObjectResult>(response.Result);
        var value = result.Value;

        var messageProperty = value.GetType().GetProperty("message");
        Assert.NotNull(messageProperty);

        var message = messageProperty.GetValue(value) as string;
        Assert.Equal("Cannot process guess", message);
    }

    [Fact]
    public void CheckGameStatus_WhenCalled_ReturnsOkObjectResult()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        gamesController.CreateGame();

        var response = gamesController.GetGame(newId);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var model = Assert.IsType<CheckGameStatusViewModel>(result.Value);
        Assert.Equal("_______", model.MaskedWord);
        Assert.Equal(5, model.AttemptsRemaining);
    }

    [Fact]
    public void DeleteGame_WhenCalled_ReturnsNoContent()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        gamesController.CreateGame();

        var response = gamesController.DeleteGame(newId);

        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public void DeleteGame_WhenCalledWithInvalidId_ReturnsNotFound()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));

        var response = gamesController.DeleteGame(newId);

        Assert.IsType<NotFoundResult>(response);
    }

    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}