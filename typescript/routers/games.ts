import { Router } from "express";
import { GamesController } from "../controllers";

const GamesRouter = Router();
GamesRouter.route("/").post(GamesController.createGame);
GamesRouter.route("/:gameId").get(GamesController.getGame);
GamesRouter.route("/:gameId").put(GamesController.makeGuess);
GamesRouter.route("/:gameId").delete(GamesController.clearGame);

export { GamesRouter };
