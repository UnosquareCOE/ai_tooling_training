package hangman;

import com.google.gson.Gson;
import hangman.controllers.GamesController;
import hangman.models.ResponseError;
import hangman.transformers.JsonTransformer;
import hangman.utils.IdentifierGenerator;

import static spark.Spark.*;

public class App {

    public static void main(String[] args) {
        var identifierGenerator = new IdentifierGenerator();
        var gamesController = new GamesController(identifierGenerator);

        after((request, response) -> response.type("application/json"));

        post("/games/", (request, response) -> gamesController.createGame());
        get("/games/:game_id", (request, response) -> gamesController.getGame(request, response), new JsonTransformer());
        post("/games/:game_id/guesses", "application/json", (request, response) -> gamesController.makeGuess(request, response), new JsonTransformer());

        // handle illegal arguments.
        exception(IllegalArgumentException.class, (e, req, res) -> {
            res.status(400);
            res.body(new Gson().toJson(new ResponseError(e)));
            res.type("application/json");
        });
    }
}