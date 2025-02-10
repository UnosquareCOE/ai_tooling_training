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
        var result = (OkObjectResult)response.Result;
        var responseModel = Assert.IsType<CreateGameResponseViewModel>(result.Value);
        Assert.Equal(newId, responseModel.GameId);
        Assert.Equal(5, responseModel.AttemptsRemaining);
        Assert.All(responseModel.MaskedWord, c => Assert.Equal('_', c));
    }

    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}