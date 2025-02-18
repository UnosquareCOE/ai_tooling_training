import {Response, Request} from "express";
import {v4 as uuid} from "uuid";
import {ERRORS, games, guessRegex, STATUS, STATUS_CODES, words} from "../constants";
import {findLetterPositions, retrieveGame, retrieveWord} from "../utils";


function createGame(req: Request, res: Response) {
    const newGameWord = retrieveWord(words);
    const newGameId = uuid();

    games[newGameId] = {
        remainingGuesses: 5,
        unmaskedWord: newGameWord,
        word: newGameWord.replaceAll(guessRegex, "_"),
        status: STATUS.IN_PROGRESS,
        incorrectGuesses: [],
    };

    res.status(STATUS_CODES.CREATED).json({
        gameId: newGameId,
        maskedWord: games[newGameId].word,
        attemptsRemaining: games[newGameId].remainingGuesses,
    });
}

function getGame(req: Request, res: Response) {
    const {gameId} = req.params;
    const game = retrieveGame(gameId, games);

    if (!game) {
        res.status(STATUS_CODES.NOT_FOUND).json({
            message: "Game not found",
        });
        return;
    }

    res.status(STATUS_CODES.OK).json({
        maskedWord: game.word,
        attemptsRemaining: game.remainingGuesses,
        guesses: game.incorrectGuesses,
        status: game.status,
    });
}


function makeGuess(req: Request, res: Response) {
    const { gameId } = req.params;
    const { letter } = req.body;

    if (!letter || letter.length != 1) {
        res.status(STATUS_CODES.BAD_REQUEST).json({
            message: "Cannot process guess",
            errors: [{
                field: "letter",
                message: ERRORS.INVALID_LETTER_LENGTH,
            }],
        });
        return;
    }

    const game = retrieveGame(gameId, games);

    if (!game) {
        res.status(STATUS_CODES.NOT_FOUND).json({
            message: "Game not found",
        });
        return;
    }

    if (game.incorrectGuesses.includes(letter) || game.word.includes(letter)) {
        res.status(STATUS_CODES.BAD_REQUEST).json({
            message: "Cannot process guess",
            errors: [{
                field: "letter",
                message: ERRORS.LETTER_ALREADY_GUESSED,
            }],
        });
        return;
    }

    const {positions, found} = findLetterPositions(game.unmaskedWord, letter);

    if (found) {
        let newMaskedWord = game.word.split('');
        positions.forEach(pos => {
            newMaskedWord[pos] = game.unmaskedWord[pos];
        });
        game.word = newMaskedWord.join('');
    } else {
        game.remainingGuesses -= 1;
        game.incorrectGuesses.push(letter);
    }

    if (game.remainingGuesses <= 0) {
        game.status = STATUS.LOST;
    } else if (game.word === game.unmaskedWord) {
        game.status = STATUS.WON;
    }

    res.status(STATUS_CODES.OK).json({
        maskedWord: game.word,
        attemptsRemaining: game.remainingGuesses,
        guesses: game.incorrectGuesses,
        status: game.status,
    });
}

function clearGame(req: Request, res: Response) {
    const {gameId} = req.params;

    if (!games[gameId]) {
        res.status(STATUS_CODES.NOT_FOUND).json({
            message: ERRORS.GAME_NOT_FOUND,
        });
        return;
    }

    delete games[gameId];

    res.status(STATUS_CODES.NO_CONTENT);
}


const GamesController = {
    createGame,
    getGame,
    makeGuess,
    clearGame
};

export {GamesController};
