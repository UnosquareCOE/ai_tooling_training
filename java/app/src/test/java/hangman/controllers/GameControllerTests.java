package hangman.controllers;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;
import static org.mockito.Mockito.verify;

import com.google.gson.Gson;
import hangman.mocks.MockIdentifierGenerator;
import hangman.models.Guess;
import hangman.models.ResponseError;
import org.junit.jupiter.api.Test;
import spark.Request;
import spark.Response;

import java.util.UUID;

public class GameControllerTests {

    @Test
    public void createGameReturnsValidUUID() {
        // arrange
        var newId = UUID.randomUUID();
        var mockIdentifierGenerator = new MockIdentifierGenerator(newId);
        var gameController = new GamesController(mockIdentifierGenerator);

        // act
        var result = gameController.createGame();

        // assert
        assertEquals(newId, result, "New game identifier is not valid.");
    }

    @Test
    public void deleteGameReturnsNoContentForInProgressGame() {
        // Arrange
        var gameId = UUID.randomUUID();
        var mockIdentifierGenerator = new MockIdentifierGenerator(gameId);
        var gameController = new GamesController(mockIdentifierGenerator);
        gameController.createGame();

        // Create mock request/response
        var mockRequest = mock(Request.class);
        var mockResponse = mock(Response.class);
        when(mockRequest.params("game_id")).thenReturn(gameId.toString());

        // Act
        var result = gameController.deleteGame(mockRequest, mockResponse);

        // Assert
        verify(mockResponse).status(204);
        assertEquals("", result);
    }

    @Test
    public void deleteGameReturnsNotFoundForNonExistentGame() {
        // Arrange
        var gameController = new GamesController(new MockIdentifierGenerator(UUID.randomUUID()));
        var nonExistentId = UUID.randomUUID();

        // Create mock request/response
        var mockRequest = mock(Request.class);
        var mockResponse = mock(Response.class);
        when(mockRequest.params("game_id")).thenReturn(nonExistentId.toString());

        // Act
        var result = gameController.deleteGame(mockRequest, mockResponse);

        // Assert
        verify(mockResponse).status(404);
        assertEquals("Game not found", ((ResponseError) result).getMessage());
    }

    @Test
    public void deleteGameReturnsBadRequestForWonGame() {
        // Arrange
        var gameId = UUID.randomUUID();
        var mockIdentifierGenerator = new MockIdentifierGenerator(gameId);
        var gameController = new GamesController(mockIdentifierGenerator);
        gameController.createGame();

        // Create mock request/response for getting the game
        var mockRequest = mock(Request.class);
        var mockResponse = mock(Response.class);
        when(mockRequest.params("game_id")).thenReturn(gameId.toString());

        // Get the game to know the word
        var game = gameController.getGame(mockRequest, mockResponse);
        String unmaskedWord = game.getUnmaskedWord();

        // Make guesses to win the game
        for (char c : unmaskedWord.toLowerCase().toCharArray()) {
            if (Character.isLetter(c)) {
                var guess = new Guess();
                guess.setLetter(String.valueOf(c));
                when(mockRequest.body()).thenReturn(new Gson().toJson(guess));
                gameController.makeGuess(mockRequest, mockResponse);
            }
        }

        // Act - Try to delete the won game
        var result = gameController.deleteGame(mockRequest, mockResponse);

        // Assert
        verify(mockResponse).status(400);
        assertEquals("Cannot delete games with status Won or Lost", ((ResponseError) result).getMessage());
    }

    @Test
    public void deleteGameReturnsBadRequestForLostGame() {
        // Arrange
        var gameId = UUID.randomUUID();
        var mockIdentifierGenerator = new MockIdentifierGenerator(gameId);
        var gameController = new GamesController(mockIdentifierGenerator);
        gameController.createGame();

        // Create mock request/response
        var mockRequest = mock(Request.class);
        var mockResponse = mock(Response.class);
        when(mockRequest.params("game_id")).thenReturn(gameId.toString());

        // Make 3 incorrect guesses to lose the game
        String[] incorrectLetters = {"z", "x", "q"};
        for (String letter : incorrectLetters) {
            var guess = new Guess();
            guess.setLetter(letter);
            when(mockRequest.body()).thenReturn(new Gson().toJson(guess));
            gameController.makeGuess(mockRequest, mockResponse);
        }

        // Act - Try to delete the lost game
        var result = gameController.deleteGame(mockRequest, mockResponse);

        // Assert
        verify(mockResponse).status(400);
        assertEquals("Cannot delete games with status Won or Lost", ((ResponseError) result).getMessage());
    }
}
