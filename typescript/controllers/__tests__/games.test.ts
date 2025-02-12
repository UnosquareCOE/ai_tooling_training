import { GamesController } from "../games";
import { Request, Response } from "express";
import { STATUS } from "../../constants/status";
import { MESSAGE } from "../../constants/message";
import { LANGUAGES } from "../../constants/languages";

interface MockRequestArgs {
  body?: any;
  params?: any;
  query?: any;
}

const mockRequest = (args?: MockRequestArgs) => {
  return {
    ...args,
  } as unknown as Request;
};

const mockResponse = () => {
  const res = {} as Response;
  res.sendStatus = jest.fn().mockReturnValue(res);
  res.status = jest.fn().mockReturnValue(res);
  res.json = jest.fn().mockReturnValue(res);
  res.send = jest.fn().mockReturnValue(res);
  return res;
};

const mockId = "fda56100-0ddb-4f06-9ea4-7c1919ff6d2f";
jest.mock("uuid", () => ({ v4: () => mockId }));

jest.mock("../games", () => ({
  ...jest.requireActual("../games"),
  getWordsFromExternalApi: jest.fn(() => Promise.resolve("mockword")),
}));

describe("GamesController", () => {
  let req: Request;
  let res: Response;

  beforeEach(() => {
    req = mockRequest();
    res = mockResponse();
  });

  describe("createGame", () => {
    it("should create a game and return a gameId with masked word", async () => {

      const newGameWord = "mockword";

    await GamesController.createGame(req, res);

      expect(res.status).toHaveBeenCalledWith(200);
      expect(res.send).toHaveBeenCalledWith({
        gameId: mockId,
        maskedWord:"______",
      });
    });
  });

  describe("getGame", () => {
    it("should return game details when the game exists", async () => {
      const gameId = mockId;
      req = mockRequest({ params: { gameId } });

      (GamesController as any).games = {
        [gameId]: {
          unmaskedWord: "mockword",
          word: "______",
          status: STATUS.IN_PROGRESS,
          incorrectGuesses: [],
          guesses: [],
        },
      };

      await GamesController.getGame(req, res);

      expect(res.status).toHaveBeenCalledWith(200);
      expect(res.json).toHaveBeenCalledWith({
        word: "______",
        status: STATUS.IN_PROGRESS,
        incorrectGuesses: [],
        guesses: [],
      });
    });

    it("should return 404 if game is not found", async () => {
      req = mockRequest({ params: { gameId: "unknown-id" } });

      await GamesController.getGame(req, res);

      expect(res.status).toHaveBeenCalledWith(404);
      expect(res.json).toHaveBeenCalledWith( MESSAGE.GAME_NOT_FOUND );
    });
  });

  describe("makeGuess", () => {
    beforeEach(() => {
      (GamesController as any).games = {
        [mockId]: {
          unmaskedWord: "mockword",
          word: "____",
          status: STATUS.IN_PROGRESS,
          incorrectGuesses: [],
          guesses: [],
        },
      };
    });

    it("should process a correct guess", async () => {
      req = mockRequest({ params: { gameId: mockId }, body: { letter: "o" } });

      await GamesController.makeGuess(req, res);

      expect(res.status).toHaveBeenCalledWith(200);
      expect(res.json).toHaveBeenCalledWith({
        word: "_o__o_",
        status: STATUS.IN_PROGRESS,
        incorrectGuesses: [],
        guesses: ["o"],
      });
    });

    it("should process an incorrect guess", async () => {
      req = mockRequest({ params: { gameId: mockId }, body: { letter: "z" } });

      await GamesController.makeGuess(req, res);

      expect(res.status).toHaveBeenCalledWith(200);
      expect(res.json).toHaveBeenCalledWith({
        word: "_o__o_",
        status: STATUS.IN_PROGRESS,
        incorrectGuesses: ["z"],
        guesses: ["o", 'z'],
      });
    });

    it("should return 400 if guess is invalid", async () => {
      req = mockRequest({ params: { gameId: mockId }, body: { letter: "zz" } });

      await GamesController.makeGuess(req, res);

      expect(res.status).toHaveBeenCalledWith(400);
      expect(res.json).toHaveBeenCalledWith({
        message: MESSAGE.MORE_THAN_ONE_LETTER,
      });
    });

    it("should return 404 if game is not found", async () => {
      req = mockRequest({ params: { gameId: "unknown-id" }, body: { letter: "o" } });

      await GamesController.makeGuess(req, res);

      expect(res.status).toHaveBeenCalledWith(404);
      expect(res.json).toHaveBeenCalledWith({ message: MESSAGE.GAME_NOT_FOUND });
    });
  });

  describe("deleteGame", () => {
    beforeEach(() => {
      (GamesController as any).games = {
        [mockId]: {
          unmaskedWord: "mockword",
          word: "____",
          status: STATUS.IN_PROGRESS,
          incorrectGuesses: [],
          guesses: [],
        },
      };
    });

    it("should delete an existing game", async () => {
      req = mockRequest({ params: { gameId: mockId } });

      await GamesController.deleteGame(req, res);

      expect(res.status).toHaveBeenCalledWith(204);

    });

    it("should return 404 if game does not exist", async () => {
      req = mockRequest({ params: { gameId: "unknown-id" } });

      await GamesController.deleteGame(req, res);

      expect(res.status).toHaveBeenCalledWith(404);
    });
  });
});
