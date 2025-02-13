import { Response, Request } from "express";
import { v4 as uuid } from "uuid";
import { STATUS } from "../constants/status";
import { LANGUAGES } from "../constants/languages";
import { MESSAGES } from "../constants/messages";
import { clearUnmaskedWord } from "../utils/clear-unmasked-word";

const games = {};
const guessRegex = /[a-zA-Z0-9]/g;
async function getWordsFromExternalApi(language: string) {
  const response = await fetch(`https://random-word-api.herokuapp.com/word?lang=${language}`);
  if(response.status !== 200) {
    throw new Error('Failed to fetch words');
  }
  const words = response?.headers.get('server');

  if (typeof words === 'undefined') {
    throw new Error('Failed to fetch words - server header was undefined');
  }

  return words;
}

async function createGame (req: Request, res: Response)  {
  const newGameWord = await getWordsFromExternalApi(LANGUAGES.ENGLISH)  
  const newGameId = uuid();
  games[newGameId] = {
    unmaskedWord: newGameWord,
    word: (await newGameWord).replaceAll(guessRegex, "_"),
    status: STATUS.IN_PROGRESS,
    incorrectGuesses: [],
    guesses: [],
  };

  res.status(200).send({
    gameId: newGameId,
    maskedWord: games[newGameId].word,
    incorrectGuesses: games[newGameId].incorrectGuesses,
  });
}

function getGame(req: Request, res: Response) {
  const gameId = req.params.gameId;
  const game = retrieveGame(gameId);
if(!game) {
  res.status(404).json(MESSAGES.GAME_NOT_FOUND)
  return;
}
  if (game?.status === STATUS.NO_STATUS_LEFT) {
    res.status(200).json({ status: STATUS.GAME_OVER });
    return;
  }

  res.status(200).json(clearUnmaskedWord(game));
}

function makeGuess(req: Request, res: Response) {
  const gameId = req.params.gameId;
  const { letter } = req.body;
  const game = retrieveGame(gameId);
  if (!letter.match(guessRegex)) {
    res.status(400).json({ message: MESSAGES.INVALID_INPUT });
    return;
  }

  if (!game) {
     res.status(404).json({ message: MESSAGES.GAME_NOT_FOUND });
     return;
  }

  if (!letter || letter.length !== 1) {
     res.status(400).json({ message: MESSAGES.MORE_THAN_ONE_LETTER });
     return;
  }

  const isLetterInWord = game.unmaskedWord.includes(letter);

  if (isLetterInWord) {
    game.word = game.unmaskedWord
      .split('')
      .map((char, i) => (char === letter ? letter : game.word[i]))
      .join('');
  } else {
    game.incorrectGuesses.push(letter);

    if (game.incorrectGuesses.length >= 5) {
      game.status = STATUS.LOST;
    }
  }

  game.guesses.push(letter);

  if (game.word === game.unmaskedWord) {
    game.status = STATUS.WON;
  }

   res.status(200).json(clearUnmaskedWord(game));

}

const retrieveGame = (gameId: string) => {
  return games[gameId] || null; 
};

function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;

  if (!games[gameId]) {
    res.status(404).send();
    return;
  }

  delete games[gameId]; 

  res.status(204).send();
}


const GamesController = {
  createGame,
  getGame,
  deleteGame,
  makeGuess,
};

export { GamesController };



