package hangman.controllers;

import com.google.gson.Gson;
import hangman.interfaces.IdentifierGeneration;
import hangman.models.Game;
import hangman.models.Guess;
import spark.Request;
import spark.Response;

import java.util.*;

public class GamesController {

    private static HashMap<UUID, Game> games = new HashMap();
    private static List<String> words = Arrays.asList("Banana", "Canine", "Unosquare", "Airport");

    private final IdentifierGeneration identifierGeneration;

    public GamesController(IdentifierGeneration identifierGeneration) {
        this.identifierGeneration = identifierGeneration;
    }

    public UUID createGame() {
        var newGameId = identifierGeneration.retrieveIdentifier();
        var newGame = new Game(3, retrieveWord());

        games.put(newGameId, newGame);

        return newGameId;
    }

    public Game getGame(Request request, Response response) {
        var gameArgument = request.params("game_id");
        var gameId = UUID.fromString(gameArgument);
        if (gameId == null || !games.containsKey(gameId)) {
            response.status(404);
            return null;
        }

        return games.get(gameId);
    }

    public Game makeGuess(Request request, Response response) {
        var game = getGame(request, response);
        if (game != null) {
            var guess = new Gson().fromJson(request.body(), Guess.class);

            if (guess == null || guess.getLetter() == null || guess.getLetter().length() != 1) {
                throw new IllegalArgumentException("Guess must be supplied with 1 letter");
            }

            // todo: add logic for making a guess, modifying the game and updating the status

            return game;
        }
        return null;
    }

    /**
     * Deletes a game that is currently in progress
     * @param request The HTTP request containing the game ID
     * @param response The HTTP response
     * @return null if successful, error message otherwise
     */
    public String deleteGame(Request request, Response response) {
        var gameArgument = request.params("game_id");
        UUID gameId;
        
        try {
            gameId = UUID.fromString(gameArgument);
        } catch (IllegalArgumentException e) {
            response.status(400);
            return "{\"error\": \"Invalid game ID\"}";
        }
        
        if (!games.containsKey(gameId)) {
            response.status(404);
            return "{\"error\": \"Game not found\"}";
        }
        
        Game game = games.get(gameId);
        if (!"In Progress".equals(game.getStatus())) {
            response.status(409);
            return "{\"error\": \"Cannot delete completed game\"}";
        }
        
        games.remove(gameId);
        response.status(204);
        return null;
    }

    private static String retrieveWord() {
        var rand = new Random();
        return words.get(rand.nextInt(words.size() - 3));
    }
}
