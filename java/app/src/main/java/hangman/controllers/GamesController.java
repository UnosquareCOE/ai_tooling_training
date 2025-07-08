package hangman.controllers;

import com.google.gson.Gson;
import hangman.interfaces.IdentifierGeneration;
import hangman.models.Game;
import hangman.models.Guess;
import hangman.models.ResponseError;
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

            String letter = guess.getLetter().toLowerCase();
            boolean isCorrectGuess = game.getUnmaskedWord().toLowerCase().contains(letter);

            if (isCorrectGuess) {
                // Update word display
                StringBuilder wordBuilder = new StringBuilder(game.getWord());
                String unmaskedWord = game.getUnmaskedWord();
                
                for (int i = 0; i < unmaskedWord.length(); i++) {
                    if (String.valueOf(unmaskedWord.charAt(i)).toLowerCase().equals(letter)) {
                        wordBuilder.setCharAt(i, unmaskedWord.charAt(i));
                    }
                }
                
                game.setWord(wordBuilder.toString());
                
                // Check if word is complete
                if (!game.getWord().contains("_")) {
                    game.setStatus("Won");
                }
            } else {
                // Add to incorrect guesses and decrease remaining
                if (!game.getIncorrectGuesses().contains(letter)) {
                    game.getIncorrectGuesses().add(letter);
                    game.setRemainingGuesses(game.getRemainingGuesses() - 1);
                }
                
                // Check if game is lost
                if (game.getRemainingGuesses() <= 0) {
                    game.setStatus("Lost");
                }
            }

            return game;
        }
        return null;
    }

    public Object deleteGame(Request request, Response response) {
        var gameArgument = request.params("game_id");
        var gameId = UUID.fromString(gameArgument);
        
        if (gameId == null || !games.containsKey(gameId)) {
            response.status(404);
            return new ResponseError("Game not found");
        }

        var game = games.get(gameId);
        if (!game.getStatus().equals("In Progress")) {
            response.status(400);
            return new ResponseError("Cannot delete games with status Won or Lost");
        }

        games.remove(gameId);
        response.status(204);
        return "";
    }

    private static String retrieveWord() {
        var rand = new Random();
        return words.get(rand.nextInt(words.size()));
    }
}
