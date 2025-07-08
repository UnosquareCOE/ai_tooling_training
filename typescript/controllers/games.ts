import { Response, Request } from "express";
import { v4 as uuid } from "uuid";

const games = {};
const words = ["banana", "canine", "unosquare", "airport"];
const guessRegex = /[a-zA-Z0-9]/g;

function createGame(req: Request, res: Response) {
  const newGameWord = retrieveWord();
  const newGameId = uuid();

  games[newGameId] = {
    remainingGuesses: 3,
    unmaskedWord: newGameWord,
    word: newGameWord.replaceAll(guessRegex, "_"),
    status: "In Progress",
    incorrectGuesses: [],
  };

  res.send(newGameId);
}

function getGame(req: Request, res: Response) {
  const { gameId } = req.params;
  const game = retrieveGame(gameId);

  res.status(200).json(clearUnmaskedWord(game));
}

function makeGuess(req: Request, res: Response) {
  const { gameId } = req.params;
  const { letter } = req.body;

  if (!letter || letter.length != 1) {
    res.status(400).json({
      message: "Letter cannot accept more than 1 character",
    });
    return;
  }

  const game = retrieveGame(gameId);

  res.status(200).json(clearUnmaskedWord(game));
}

/**
 * Deletes a game that is currently in progress
 * @param req Express request object containing gameId parameter
 * @param res Express response object
 */
function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;

  if (!gameId) {
    return res.status(400).json({
      error: "Invalid game ID",
    });
  }

  const game = retrieveGame(gameId);

  if (!game) {
    return res.status(404).json({
      error: "Game not found",
    });
  }

  if (game.status !== "In Progress") {
    return res.status(409).json({
      error: "Cannot delete completed game",
    });
  }

  delete games[gameId];
  res.status(204).send();
}

const retrieveGame = (gameId: string) => games[gameId];

const retrieveWord = () => words[Math.ceil(1 * words.length - 1)];

const clearUnmaskedWord = (game: any) => {
  const withoutUnmasked = {
    ...game,
  };
  delete withoutUnmasked.unmaskedWord;
  return withoutUnmasked;
};

const GamesController = {
  createGame,
  getGame,
  makeGuess,
  deleteGame,
};

export { GamesController };
