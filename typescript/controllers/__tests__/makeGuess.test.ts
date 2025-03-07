import { GamesController } from "../games";
import { Request, Response } from "express";
import {ERRORS, games, STATUS, STATUS_CODES} from "../../constants";
import { v4 as uuid } from "uuid";

jest.mock("uuid", () => ({ v4: jest.fn() }));
jest.mock("../../constants", () => ({
    words: ["apple", "banana", "cherry"],
    games: {},
    guessRegex: /[a-zA-Z]/g,
    STATUS: {
        IN_PROGRESS: "IN_PROGRESS",
        WON: "WON",
        LOST: "LOST",
    },
    STATUS_CODES: {
        CREATED: 201,
        OK: 200,
        NOT_FOUND: 404,
        BAD_REQUEST: 400,
    },
    ERRORS: {
        INVALID_LETTER_LENGTH: "Invalid letter length",
    },
}));

const mockRequest = (args?: any) => {
    return {
        ...args,
    } as unknown as Request;
};

const mockResponse = () => {
    const res = {} as Response;
    res.status = jest.fn().mockReturnValue(res);
    res.json = jest.fn().mockReturnValue(res);
    return res;
};

describe("GamesController", () => {
    describe("makeGuess", () => {
        beforeEach(() => {
            const mockId = "test-id";
            (uuid as jest.Mock).mockReturnValue(mockId);
            games[mockId] = {
                remainingGuesses: 5,
                unmaskedWord: "apple",
                word: "_____",
                status: STATUS.IN_PROGRESS,
                incorrectGuesses: [],
            };
        });

        it("should return an error if the guessed letter is not a valid alphabet character", () => {
            const req = mockRequest({ params: { gameId: "test-id" }, body: { letter: "1" } });
            const res = mockResponse();

            expect(() => GamesController.makeGuess(req, res)).toThrow(ERRORS.SINGLE_ALPHABETIC_CHARACTER);
        });

        it("should return an error if the guessed letter has been guessed before", () => {
            const req = mockRequest({ params: { gameId: "test-id" }, body: { letter: "a" } });
            const res = mockResponse();

            GamesController.makeGuess(req, res); // First guess
            GamesController.makeGuess(req, res); // Second guess

            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.BAD_REQUEST);
            expect(res.json).toHaveBeenCalledWith({
                message: "Cannot process guess",
                errors: [{ field: "letter", message: ERRORS.LETTER_ALREADY_GUESSED }],
            });
        });

        it("should handle both upper and lower case characters", () => {
            const reqLower = mockRequest({ params: { gameId: "test-id" }, body: { letter: "a" } });
            const reqUpper = mockRequest({ params: { gameId: "test-id" }, body: { letter: "A" } });
            const res = mockResponse();

            GamesController.makeGuess(reqLower, res);
            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.OK);
            expect(res.json).toHaveBeenCalledWith(expect.objectContaining({
                maskedWord: "a____",
                attemptsRemaining: 5,
                guesses: [],
                status: STATUS.IN_PROGRESS,
            }));

            GamesController.makeGuess(reqUpper, res);
            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.OK);
            expect(res.json).toHaveBeenCalledWith(expect.objectContaining({
                maskedWord: "a____",
                attemptsRemaining: 5,
                guesses: [],
                status: STATUS.IN_PROGRESS,
            }));
        });

        it("should handle the game being won", () => {
            const req = mockRequest({ params: { gameId: "test-id" }, body: { letter: "a" } });
            const res = mockResponse();

            GamesController.makeGuess(req, res); // Guess 'a'
            req.body.letter = "p";
            GamesController.makeGuess(req, res); // Guess 'p'
            req.body.letter = "l";
            GamesController.makeGuess(req, res); // Guess 'l'
            req.body.letter = "e";
            GamesController.makeGuess(req, res); // Guess 'e'

            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.OK);
            expect(res.json).toHaveBeenCalledWith(expect.objectContaining({
                maskedWord: "apple",
                attemptsRemaining: 5,
                guesses: [],
                status: STATUS.WON,
            }));
        });

        it("should handle the game being lost", () => {
            const req = mockRequest({ params: { gameId: "test-id" }, body: { letter: "z" } });
            const res = mockResponse();
            const letters = ["z", "x", "q", "w", "r"];

            for (let i = 0; i < 5; i++) {
                req.body.letter = letters[i];
                GamesController.makeGuess(req, res); // Incorrect guess
            }

            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.OK);
            expect(res.json).toHaveBeenCalledWith(expect.objectContaining({
                maskedWord: "_____",
                attemptsRemaining: 0,
                guesses: ["z", "x", "q", "w", "r"],
                status: STATUS.LOST,
            }));
        });
    });
});