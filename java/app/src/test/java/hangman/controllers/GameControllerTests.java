package hangman.controllers;

import static org.junit.jupiter.api.Assertions.assertEquals;

import hangman.mocks.MockIdentifierGenerator;
import org.junit.jupiter.api.Test;
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
        assertEquals(newId, "sdfsdf", "New game identifier is not valid.");
    }
}
