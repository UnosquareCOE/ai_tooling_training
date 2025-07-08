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

  const lowerLetter = letter.toLowerCase();
  const isCorrectGuess = game.unmaskedWord.toLowerCase().includes(lowerLetter);

  if (isCorrectGuess) {
    // Update word display
    let wordArray = game.word.split('');
    const unmaskedArray = game.unmaskedWord.split('');
    
    for (let i = 0; i < unmaskedArray.length; i++) {
      if (unmaskedArray[i].toLowerCase() === lowerLetter) {
        wordArray[i] = unmaskedArray[i];
      }
    }
    
    game.word = wordArray.join('');
    
    // Check if word is complete
    if (!game.word.includes('_')) {
      game.status = "Won";
    }
  } else {
    // Add to incorrect guesses and decrease remaining
    if (!game.incorrectGuesses.includes(lowerLetter)) {
      game.incorrectGuesses.push(lowerLetter);
      game.remainingGuesses--;
    }
    
    // Check if game is lost
    if (game.remainingGuesses <= 0) {
      game.status = "Lost";
    }
  }

  res.status(200).json(clearUnmaskedWord(game));
}

function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;
  const game = retrieveGame(gameId);

  if (!game) {
    res.status(404).json({
      message: "Game not found",
    });
    return;
  }

  if (game.status !== "In Progress") {
    res.status(400).json({
      message: "Cannot delete games with status Won or Lost",
    });
    return;
  }

  delete games[gameId];
  res.status(204).send();
}

const retrieveGame = (gameId: string) => games[gameId];

const retrieveWord = () => words[Math.floor(Math.random() * words.length)];

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
