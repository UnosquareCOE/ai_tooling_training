import {Response, Request} from "express";
import {v4 as uuid} from "uuid";
import {ERRORS, games, guessRegex, STATUS, STATUS_CODES, words} from "../constants";
import {clearUnmaskedWord, retrieveGame, retrieveWord} from "../utils";


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

    res.status(STATUS_CODES.OK).json(clearUnmaskedWord(game));
}

function makeGuess(req: Request, res: Response) {
    const {gameId} = req.params;
    const {letter} = req.body;

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
            message: "Game not found",
        });
        return;
    }

    delete games[gameId];

    res.status(STATUS_CODES.OK).json({
        message: "Game deleted successfully",
    });

}


const GamesController = {
    createGame,
    getGame,
    makeGuess,
    clearGame
};

export {GamesController};
