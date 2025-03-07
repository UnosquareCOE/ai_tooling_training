import { GamesController } from "../games";
import { Request, Response } from "express";
import { words, games } from "../../constants";
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
    describe("createGame", () => {
        it("should randomly select a word from the predefined list", () => {
            const req = mockRequest();
            const res = mockResponse();
            const mockId = "test-id";
            (uuid as jest.Mock).mockReturnValue(mockId);

            GamesController.createGame(req, res);

            const selectedWord = games[mockId].unmaskedWord;
            expect(words).toContain(selectedWord);
        });

        it("should store the selected word and its masked version in the game state", () => {
            const req = mockRequest();
            const res = mockResponse();
            const mockId = "test-id";
            (uuid as jest.Mock).mockReturnValue(mockId);

            GamesController.createGame(req, res);

            const gameState = games[mockId];
            expect(gameState).toBeDefined();
            expect(gameState.unmaskedWord).toBeDefined();
            expect(gameState.word).toBe(gameState.unmaskedWord.replace(/[a-zA-Z]/g, "_"));
        });
    });
});