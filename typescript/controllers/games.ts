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

  if (!game) {
    res.status(404).json({
      message: "Game not found",
    });
    return;
  }

  // Check if game is already over
  if (game.status !== "In Progress") {
    res.status(400).json({
      message: "Game is already finished",
    });
    return;
  }

  // Process the guess
  const lowerLetter = letter.toLowerCase();
  const lowerUnmaskedWord = game.unmaskedWord.toLowerCase();
  
  if (lowerUnmaskedWord.includes(lowerLetter)) {
    // Correct guess - update masked word
    let newWord = "";
    for (let i = 0; i < game.unmaskedWord.length; i++) {
      if (lowerUnmaskedWord[i] === lowerLetter) {
        newWord += game.unmaskedWord[i];
      } else {
        newWord += game.word[i];
      }
    }
    game.word = newWord;

    // Check if word is fully guessed (won)
    if (!game.word.includes("_")) {
      game.status = "Won";
    }
  } else {
    // Incorrect guess
    if (!game.incorrectGuesses.includes(letter)) {
      game.incorrectGuesses.push(letter);
      game.remainingGuesses--;

      // Check if out of guesses (lost)
      if (game.remainingGuesses <= 0) {
        game.status = "Lost";
      }
    }
  }

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

function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;
  const game = retrieveGame(gameId);

  if (!game) {
    res.status(404).json({ message: "Game not found" });
    return;
  }

  if (game.status !== "In Progress") {
    res.status(400).json({
      message: "Only games with status 'In Progress' can be deleted",
    });
    return;
  }

  delete games[gameId];
  res.status(204).send();
}

const GamesController = {
  createGame,
  getGame,
  makeGuess,
  deleteGame,
};

export { GamesController };
