import { GamesController } from "../games";
import { Request, Response } from "express";

interface mockRequestArgs {
  body?: any;
  params?: any;
  query?: any;
  headers?: any;
  token?: string;
  locals?: any;
}

const mockRequest = (args?: mockRequestArgs) => {
  return {
    ...args,
  } as unknown as Request;
};

const mockResponse = (userId?: number) => {
  const res = {} as Response;
  res.sendStatus = jest.fn().mockReturnValue(res);
  res.status = jest.fn().mockReturnValue(res);
  res.json = jest.fn().mockReturnValue(res);
  res.send = jest.fn().mockReturnValue(res);
  return res;
};

const mockId = 'fda56100-0ddb-4f06-9ea4-7c1919ff6d2f';
jest.mock("uuid", () => ({ v4: () => mockId }));

describe("game controller", () => {
    describe("createGame", () => {
      it("Should return identifier when game created", () => {
        const req = mockRequest();
        const res = mockResponse();

        GamesController.createGame(req, res);

        expect(res.send).toHaveBeenCalledTimes(1);
        expect(res.send).toHaveBeenCalledWith(mockId);
      });
    });

    describe("deleteGame", () => {
      it("Should return 404 when game not found", () => {
        const req = mockRequest({ params: { gameId: "nonexistent" } });
        const res = mockResponse();

        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(404);
        expect(res.json).toHaveBeenCalledWith({
          message: "Game not found"
        });
      });

      it("Should return 204 when deleting in-progress game", () => {
        // Create a game first
        const createReq = mockRequest();
        const createRes = mockResponse();
        GamesController.createGame(createReq, createRes);

        // Delete the game
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();
        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(204);
        expect(res.send).toHaveBeenCalledWith();
      });

      it("Should return 400 when trying to delete Won game", () => {
        // Create a game first
        const createReq = mockRequest();
        const createRes = mockResponse();
        GamesController.createGame(createReq, createRes);

        // Get the game to know what word it is
        const getReq = mockRequest({ params: { gameId: mockId } });
        const getRes = mockResponse();
        GamesController.getGame(getReq, getRes);
        
        // Extract the word from the mock calls - it will be one of the predefined words
        // Make guesses to win the game - we'll guess common letters that should win any of the words
        const commonLetters = ['a', 'n', 'i', 'o', 'e', 'r', 'p', 't', 'c', 's', 'u', 'q', 'b'];
        commonLetters.forEach(letter => {
          const guessReq = mockRequest({ 
            params: { gameId: mockId },
            body: { letter }
          });
          const guessRes = mockResponse();
          GamesController.makeGuess(guessReq, guessRes);
        });

        // Try to delete the won game
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();
        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(400);
        expect(res.json).toHaveBeenCalledWith({
          message: "Cannot delete games with status Won or Lost"
        });
      });

      it("Should return 400 when trying to delete Lost game", () => {
        // Create a game first
        const createReq = mockRequest();
        const createRes = mockResponse();
        GamesController.createGame(createReq, createRes);

        // Make 3 incorrect guesses to lose the game
        const incorrectLetters = ['z', 'x', 'q'];
        incorrectLetters.forEach(letter => {
          const guessReq = mockRequest({ 
            params: { gameId: mockId },
            body: { letter }
          });
          const guessRes = mockResponse();
          GamesController.makeGuess(guessReq, guessRes);
        });

        // Try to delete the lost game
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();
        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(400);
        expect(res.json).toHaveBeenCalledWith({
          message: "Cannot delete games with status Won or Lost"
        });
      });
    });
});