using api.Controllers;
using api.ViewModels;
using api.Constants;
using Microsoft.AspNetCore.Mvc;
using api.Utils;

namespace api.test.Controllers;

public class MockIdentifierGenerator : IIdentifierGenerator
{
    private readonly Guid _newId;

    public MockIdentifierGenerator(Guid newId)
    {
        _newId = newId;
    }

    public Guid RetrieveIdentifier()
    {
        return _newId;
    }
}

public class GamesControllerTests
{
    [Fact]
    public async Task CreateGame_WhenCalled_ReturnsValidIdentifier()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var request = new CreateGameRequestViewModel { Language = "en" };

        var response = await gamesController.CreateGame(request);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<CreateGameResponseViewModel>(result.Value);
        Assert.Equal(newId, responseValue.GameId);
    }
        
    [Fact]
    public async Task GetGame_WhenGameExists_ReturnsGame()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var request = new CreateGameRequestViewModel { Language = "en" };
        
        var createResponse = await gamesController.CreateGame(request);
        var createResult = Assert.IsType<OkObjectResult>(createResponse.Result);
        var createResponseValue = Assert.IsType<CreateGameResponseViewModel>(createResult.Value);
        var gameId = createResponseValue.GameId;

        var response = gamesController.GetGame(gameId);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<MakeGuessResponseViewModel>(result.Value);
        Assert.Equal("_______", responseValue.MaskedWord);
        Assert.Equal(5, responseValue.AttemptsRemaining);
        Assert.Empty(responseValue.Guesses);
        Assert.Equal(GameStatuses.InProgress.ToString(), responseValue.Status);
    }
        
        
    [Fact]
    public async Task MakeGuess_WhenGameExistsAndValidGuess_ReturnsUpdatedGame()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var request = new CreateGameRequestViewModel { Language = "en" };
        
        var createResponse = await gamesController.CreateGame(request);
        var createResult = Assert.IsType<OkObjectResult>(createResponse.Result);
        var createResponseValue = Assert.IsType<CreateGameResponseViewModel>(createResult.Value);
        var gameId = createResponseValue.GameId;
        
        var cheatResponse = gamesController.Cheat(gameId);
        var cheatResult = Assert.IsType<OkObjectResult>(cheatResponse.Result);
        var unmaskedWord = Assert.IsType<string>(cheatResult.Value);

        var guess = new GuessViewModel { Letter = unmaskedWord[0].ToString() };
        var response = gamesController.MakeGuess(gameId, guess);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<MakeGuessResponseViewModel>(result.Value);
        Assert.Equal($"{unmaskedWord[0]}______", responseValue.MaskedWord);
        Assert.Equal(5, responseValue.AttemptsRemaining);
        Assert.Empty(responseValue.Guesses);
        Assert.Equal(GameStatuses.InProgress.ToString(), responseValue.Status);
    }
        
    [Fact]
    public async Task MakeGuess_WhenGameExistsAndInvalidGuess_ReturnsUpdatedGame()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var request = new CreateGameRequestViewModel { Language = "en" };
        
        var createResponse = await gamesController.CreateGame(request);
        var createResult = Assert.IsType<OkObjectResult>(createResponse.Result);
        var createResponseValue = Assert.IsType<CreateGameResponseViewModel>(createResult.Value);
        var gameId = createResponseValue.GameId;
            
        var guess = new GuessViewModel { Letter = "0" };
        var response = gamesController.MakeGuess(gameId, guess);

        var result = Assert.IsType<OkObjectResult>(response.Result);
        var responseValue = Assert.IsType<MakeGuessResponseViewModel>(result.Value);
        Assert.Equal(4, responseValue.AttemptsRemaining);
        Assert.Equal(GameStatuses.InProgress.ToString(), responseValue.Status);
    }
    
    [Fact]
    public async Task DeleteGame_WhenGameExists_ReturnsNoContent()
    {
        var newId = Guid.NewGuid();
        var gamesController = RetrieveController(new MockIdentifierGenerator(newId));
        var request = new CreateGameRequestViewModel { Language = "en" };
        
        var createResponse = await gamesController.CreateGame(request);
        var createResult = Assert.IsType<OkObjectResult>(createResponse.Result);
        var createResponseValue = Assert.IsType<CreateGameResponseViewModel>(createResult.Value);
        var gameId = createResponseValue.GameId;
        
        var deleteResponse = gamesController.DeleteGame(gameId);

        Assert.IsType<NoContentResult>(deleteResponse);
    }

    [Fact]
    public void DeleteGame_WhenGameDoesNotExist_ReturnsNotFound()
    {
        var gamesController = RetrieveController(new MockIdentifierGenerator(Guid.NewGuid()));
        var nonExistentGameId = Guid.NewGuid();
        
        var deleteResponse = gamesController.DeleteGame(nonExistentGameId);

        var result = Assert.IsType<NotFoundObjectResult>(deleteResponse);
        var responseValue = Assert.IsType<ResponseErrorViewModel>(result.Value);
        Assert.Equal("Game not found", responseValue.Message);
        Assert.Single(responseValue.Errors);
        Assert.Equal("gameId", responseValue.Errors[0].Field);
        Assert.Equal("The specified game ID does not exist.", responseValue.Errors[0].Message);
    }
        
    private static GamesController RetrieveController(IIdentifierGenerator identifierGenerator)
    {
        return new GamesController(identifierGenerator);
    }
}