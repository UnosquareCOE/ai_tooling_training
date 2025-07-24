package hangman.controllers;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import hangman.mocks.MockIdentifierGenerator;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.BeforeEach;
import spark.Request;
import spark.Response;
import java.util.UUID;
import java.lang.reflect.Field;
import java.util.HashMap;
import hangman.models.Game;

public class GameControllerTests {

    private GamesController gameController;
    private Request mockRequest;
    private Response mockResponse;
    private UUID testGameId;

    @BeforeEach
    public void setUp() throws Exception {
        // Clear the static games HashMap before each test
        Field gamesField = GamesController.class.getDeclaredField("games");
        gamesField.setAccessible(true);
        HashMap<UUID, Game> games = (HashMap<UUID, Game>) gamesField.get(null);
        games.clear();

        testGameId = UUID.randomUUID();
        gameController = new GamesController(new MockIdentifierGenerator(testGameId));
        mockRequest = mock(Request.class);
        mockResponse = mock(Response.class);
    }

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
    public void deleteGame_WithInProgressGame_ReturnsEmptyStringAnd204() {
        // arrange
        gameController.createGame();
        when(mockRequest.params("game_id")).thenReturn(testGameId.toString());

        // act
        var result = gameController.deleteGame(mockRequest, mockResponse);

        // assert
        verify(mockResponse).status(204);
        assertEquals("", result);
        
        // verify game is deleted
        when(mockRequest.params("game_id")).thenReturn(testGameId.toString());
        var getResult = gameController.getGame(mockRequest, mockResponse);
        verify(mockResponse).status(404);
        assertNull(getResult);
    }

    @Test
    public void deleteGame_WithNonExistentGame_Returns404() {
        // arrange
        var nonExistentId = UUID.randomUUID();
        when(mockRequest.params("game_id")).thenReturn(nonExistentId.toString());

        // act
        var result = gameController.deleteGame(mockRequest, mockResponse);

        // assert
        verify(mockResponse).status(404);
        assertEquals("", result);
    }

    @Test
    public void deleteGame_WithWonGame_Throws400Exception() throws Exception {
        // arrange
        gameController.createGame();
        
        // Access and modify game status
        Field gamesField = GamesController.class.getDeclaredField("games");
        gamesField.setAccessible(true);
        HashMap<UUID, Game> games = (HashMap<UUID, Game>) gamesField.get(null);
        Game game = games.get(testGameId);
        
        Field statusField = Game.class.getDeclaredField("status");
        statusField.setAccessible(true);
        statusField.set(game, "Won");
        
        when(mockRequest.params("game_id")).thenReturn(testGameId.toString());

        // act & assert
        IllegalArgumentException exception = assertThrows(IllegalArgumentException.class, () -> {
            gameController.deleteGame(mockRequest, mockResponse);
        });
        
        assertEquals("Only games with status 'In Progress' can be deleted", exception.getMessage());
        verify(mockResponse).status(400);
    }

    @Test
    public void deleteGame_WithLostGame_Throws400Exception() throws Exception {
        // arrange
        gameController.createGame();
        
        // Access and modify game status
        Field gamesField = GamesController.class.getDeclaredField("games");
        gamesField.setAccessible(true);
        HashMap<UUID, Game> games = (HashMap<UUID, Game>) gamesField.get(null);
        Game game = games.get(testGameId);
        
        Field statusField = Game.class.getDeclaredField("status");
        statusField.setAccessible(true);
        statusField.set(game, "Lost");
        
        when(mockRequest.params("game_id")).thenReturn(testGameId.toString());

        // act & assert
        IllegalArgumentException exception = assertThrows(IllegalArgumentException.class, () -> {
            gameController.deleteGame(mockRequest, mockResponse);
        });
        
        assertEquals("Only games with status 'In Progress' can be deleted", exception.getMessage());
        verify(mockResponse).status(400);
    }

    @Test
    public void deleteGame_WithInvalidUUID_ThrowsException() {
        // arrange
        when(mockRequest.params("game_id")).thenReturn("invalid-uuid");

        // act & assert
        assertThrows(IllegalArgumentException.class, () -> {
            gameController.deleteGame(mockRequest, mockResponse);
        });
    }
}
