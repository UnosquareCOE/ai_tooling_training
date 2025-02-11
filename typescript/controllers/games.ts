import { Response, Request } from "express";
import { v4 as uuid } from "uuid";

const games = {};
const guessRegex = /[a-zA-Z0-9]/g;
async function getWordsFromExternalApi(language: string) {
  const response = await fetch(`https://random-word-api.herokuapp.com/word?lang=${language}`);
  const words = response?.headers.get('server');
  return words;
}
async function createGame (req: Request, res: Response)  {
  const newGameWord = await getWordsFromExternalApi("en")  
  const newGameId = uuid();
  games[newGameId] = {
    unmaskedWord: newGameWord,
    word: (await newGameWord).replaceAll(guessRegex, "_"),
    status: "In Progress",
    incorrectGuesses: [],
    guesses: [],
  };

  res.status(201).send({
    gameId: newGameId,
    maskedWord: games[newGameId].word,
    attemptsRemaining: games[newGameId].remainingGuesses,
  });
}

function getGame(req: Request, res: Response) {
  const { gameId } = req.params;
  const game = retrieveGame(gameId);

  if (game.status === "no guesses left") {
    res.status(200).json({
      status: "game over",
    });
    return;
  }

  res.status(200).json(clearUnmaskedWord(game));
}

function makeGuess(req: Request, res: Response) {
  const { gameId } = req.params;
  const { letter } = req.body;

  if (!letter || letter.length !== 1) {
    res.status(400).json({
      message: "Letter cannot accept more than 1 character",
    });
    return;
  }

  const game = retrieveGame(gameId);
  const word = game.word;
  const unmaskedWord = game.unmaskedWord;

  game.guesses.push(letter);

  if (!isValidGuess(letter)) {
    res.status(400).json({
      message: "Cannot process guess",
      errors: [
        {
          field: "letter",
          message: "Letter cannot accept more than 1 character",
        },
      ],
    });
    return;
  }

  if (typeof unmaskedWord === "string" && unmaskedWord.includes(letter)) {
    let newWord = "";
    for (let i = 0; i < unmaskedWord.length; i++) {
      if (unmaskedWord[i] === letter) {
        newWord += letter;
      } else {
        newWord += word[i];
      }
    }
    game.word = newWord;
  } else {
    game.incorrectGuesses.push(letter);
    game.remainingGuesses -= 1;

    if (game.remainingGuesses <= 0) {
      game.status = "game over";
      delete games[gameId];
    }
  }

  if (game.status === "game over") {
    res.status(200).json({
      status: "game over",
    });
    return;
  }

  if (game.word === game.unmaskedWord) {
    game.status = "congrats you won";
    res.status(200).json({
      gameId,
      unmaskedWord,
      status: "won",
    });
    return;
  }
  res.status(200).json(clearUnmaskedWord(game));
}

const retrieveGame = (gameId: string) => {
  if (!games[gameId]) {
    throw new Error("Game not found or already deleted");
  }
  return games[gameId];
};

const clearUnmaskedWord = (game: any) => {
  const withoutUnmasked = {
    ...game,
  };
  delete withoutUnmasked.unmaskedWord;
  return withoutUnmasked;
};
function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;

  if (!games[gameId]) {
    res.status(404).send();
    return;
  }

  delete games[gameId];
  res.status(204).send();
  return; // Add this line to return void
}
const isValidGuess = (guess: string) => guess.length === 1;

const GamesController = {
  createGame,
  getGame,
  deleteGame,
  makeGuess,
};

export { GamesController };



