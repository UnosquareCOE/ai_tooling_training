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
};

export { GamesController };
