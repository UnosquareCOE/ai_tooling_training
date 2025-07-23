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

            // Check if game is already over
            if (!game.getStatus().equals("In Progress")) {
                throw new IllegalArgumentException("Game is already finished");
            }

            // Process the guess
            var letter = guess.getLetter();
            var lowerLetter = letter.toLowerCase();
            var lowerUnmaskedWord = game.getUnmaskedWord().toLowerCase();

            if (lowerUnmaskedWord.contains(lowerLetter)) {
                // Correct guess - update masked word
                var newWord = new StringBuilder();
                for (int i = 0; i < game.getUnmaskedWord().length(); i++) {
                    if (lowerUnmaskedWord.charAt(i) == lowerLetter.charAt(0)) {
                        newWord.append(game.getUnmaskedWord().charAt(i));
                    } else {
                        newWord.append(game.getWord().charAt(i));
                    }
                }
                game.setWord(newWord.toString());

                // Check if word is fully guessed (won)
                if (!game.getWord().contains("_")) {
                    game.setStatus("Won");
                }
            } else {
                // Incorrect guess
                if (!game.getIncorrectGuesses().contains(letter)) {
                    game.getIncorrectGuesses().add(letter);
                    game.setRemainingGuesses(game.getRemainingGuesses() - 1);

                    // Check if out of guesses (lost)
                    if (game.getRemainingGuesses() <= 0) {
                        game.setStatus("Lost");
                    }
                }
            }

            return game;
        }
        return null;
    }

    public String deleteGame(Request request, Response response) {
        var gameArgument = request.params("game_id");
        var gameId = UUID.fromString(gameArgument);
        
        if (!games.containsKey(gameId)) {
            response.status(404);
            return new Gson().toJson(new ResponseError("Game not found"));
        }
        
        var game = games.get(gameId);
        if (!game.getStatus().equals("In Progress")) {
            response.status(400);
            return new Gson().toJson(new ResponseError(
                "Only games with status 'In Progress' can be deleted"
            ));
        }
        
        games.remove(gameId);
        response.status(204);
        return "";
    }

    private static String retrieveWord() {
        var rand = new Random();
        return words.get(rand.nextInt(words.size() - 3));
    }
}
