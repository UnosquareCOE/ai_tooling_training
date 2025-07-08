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
      it("Should return 204 when deleting in progress game", () => {
        // Arrange
        const req = mockRequest({ params: { gameId: mockId } });
        const res = mockResponse();
        
        // Create a game first
        GamesController.createGame(mockRequest(), mockResponse());
        
        // Act
        GamesController.deleteGame(req, res);
        
        // Assert
        expect(res.status).toHaveBeenCalledWith(204);
        expect(res.send).toHaveBeenCalledTimes(1);
      });
      
      it("Should return 404 when game not found", () => {
        // Arrange
        const req = mockRequest({ params: { gameId: "non-existent-id" } });
        const res = mockResponse();
        
        // Act
        GamesController.deleteGame(req, res);
        
        // Assert
        expect(res.status).toHaveBeenCalledWith(404);
        expect(res.json).toHaveBeenCalledWith({
          error: "Game not found"
        });
      });
      
      it("Should return 400 when game ID is missing", () => {
        // Arrange
        const req = mockRequest({ params: {} });
        const res = mockResponse();
        
        // Act
        GamesController.deleteGame(req, res);
        
        // Assert
        expect(res.status).toHaveBeenCalledWith(400);
        expect(res.json).toHaveBeenCalledWith({
          error: "Invalid game ID"
        });
      });
    });
});