import { GamesController, games } from "../games";
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
    beforeEach(() => {
      // Clear games between tests
      Object.keys(games).forEach(key => delete games[key]);
    });

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
      it("Should delete an in-progress game and return 204", () => {
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();

        // Create a game first
        GamesController.createGame(mockRequest(), mockResponse());

        // Delete the game
        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(204);
        expect(res.send).toHaveBeenCalledTimes(1);

        // Verify game is deleted
        const getReq = mockRequest({ params: { gameId: mockId } });
        const getRes = mockResponse();
        GamesController.getGame(getReq, getRes);
        
        expect(getRes.status).toHaveBeenCalledWith(404);
        expect(getRes.send).toHaveBeenCalledTimes(1);
      });

      it("Should return 404 for non-existent game", () => {
        const req = mockRequest({ params: { gameId: 'non-existent-id' } });
        const res = mockResponse();

        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(404);
        expect(res.send).toHaveBeenCalledTimes(1);
      });

      it("Should return 400 when trying to delete a won game", () => {
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();

        // Create a game
        GamesController.createGame(mockRequest(), mockResponse());

        // Modify game status to Won
        games[mockId].status = "Won";

        // Try to delete the won game
        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(400);
        expect(res.json).toHaveBeenCalledWith({
          message: "Only games with status 'In Progress' can be deleted"
        });
      });

      it("Should return 400 when trying to delete a lost game", () => {
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();

        // Create a game
        GamesController.createGame(mockRequest(), mockResponse());

        // Modify game status to Lost
        games[mockId].status = "Lost";

        // Try to delete the lost game
        GamesController.deleteGame(req, res);

        expect(res.status).toHaveBeenCalledWith(400);
        expect(res.json).toHaveBeenCalledWith({
          message: "Only games with status 'In Progress' can be deleted"
        });
      });
    });
});