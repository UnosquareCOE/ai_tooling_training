import { GamesController } from "../games";
import { Request, Response } from "express";
import {games, ERRORS, STATUS_CODES, STATUS} from "../../constants";
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
        GAME_NOT_FOUND: "Game not found",
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
    describe("getGame", () => {
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

        it("should return the game details if the game ID is valid", () => {
            const req = mockRequest({ params: { gameId: "test-id" } });
            const res = mockResponse();

            GamesController.getGame(req, res);

            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.OK);
            expect(res.json).toHaveBeenCalledWith({
                maskedWord: "_____",
                attemptsRemaining: 5,
                guesses: [],
                status: STATUS.IN_PROGRESS,
            });
        });

        it("should return an error if the game ID is invalid", () => {
            const req = mockRequest({ params: { gameId: "invalid-id" } });
            const res = mockResponse();

            GamesController.getGame(req, res);

            expect(res.status).toHaveBeenCalledWith(STATUS_CODES.NOT_FOUND);
            expect(res.json).toHaveBeenCalledWith({
                message: ERRORS.GAME_NOT_FOUND,
            });
        });
    });
});