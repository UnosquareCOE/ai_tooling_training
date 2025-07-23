# HE-8: Add Delete Game API - Implementation Plan

## Overview
This plan outlines the implementation of a DELETE API endpoint to remove in-progress games from the Hangman game application. The endpoint will be available at `/games/{gameId}` and will only allow deletion of games with status "In Progress".

## Project Context
- **Tech Stacks**: C# (.NET), Java (Spark), TypeScript (Express)
- **Storage**: In-memory (no persistence)
- **Current Endpoints**:
  - POST /games - Create new game
  - GET /games/{gameId} - Get game status
  - PUT /games/{gameId} - Make a guess

## Implementation Tasks

### 1. C# (.NET) Implementation
- [ ] Add DELETE endpoint to `GamesController.cs`
  - [ ] Implement `[HttpDelete("{gameId:guid}")]` method
  - [ ] Check if game exists
  - [ ] Verify game status is "In Progress"
  - [ ] Remove game from Dictionary
  - [ ] Return appropriate HTTP status codes
- [ ] Add unit tests to `GamesControllerTests.cs`
  - [ ] Test successful deletion of in-progress game
  - [ ] Test rejection of won game deletion
  - [ ] Test rejection of lost game deletion
  - [ ] Test 404 for non-existent game
  - [ ] Test invalid GUID format

### 2. Java (Spark) Implementation
- [ ] Add DELETE route to `App.java`
  - [ ] Implement `delete("/games/:game_id", ...)` route
  - [ ] Parse and validate UUID
  - [ ] Check game existence and status
  - [ ] Remove from HashMap
  - [ ] Return appropriate responses
- [ ] Update `GamesController.java` with delete logic
  - [ ] Add `deleteGame(UUID gameId)` method
  - [ ] Implement status validation
- [ ] Add tests to `GameControllerTests.java`
  - [ ] Test successful deletion
  - [ ] Test status-based rejections
  - [ ] Test invalid UUID handling
  - [ ] Test non-existent game

### 3. TypeScript (Express) Implementation
- [ ] Add DELETE route to `routers/games.ts`
  - [ ] Implement `router.delete('/:gameId', ...)` route
- [ ] Add delete handler to `controllers/games.ts`
  - [ ] Implement `deleteGame` function
  - [ ] Validate game existence
  - [ ] Check game status
  - [ ] Delete from games object
  - [ ] Return appropriate responses
- [ ] Add tests to `controllers/__tests__/games.test.ts`
  - [ ] Test successful deletion
  - [ ] Test won/lost game rejection
  - [ ] Test missing game handling
  - [ ] Test invalid gameId format

### 4. API Documentation & Testing
- [ ] Update Postman collection (`tests/postman.api.json`)
  - [ ] Add DELETE /games/{gameId} request
  - [ ] Add test scenarios for different game states
- [ ] Update any API documentation or README files

### 5. Cross-cutting Concerns
- [ ] Ensure consistent error messages across all implementations
- [ ] Verify HTTP status codes are consistent:
  - 204 No Content - Successful deletion
  - 400 Bad Request - Game is Won or Lost
  - 404 Not Found - Game doesn't exist
- [ ] Consider adding logging for delete operations

## Test Coverage Requirements
- Maintain or improve existing test coverage
- Each implementation should have at minimum:
  - Happy path test (delete in-progress game)
  - Negative tests (won/lost games)
  - Edge cases (non-existent, invalid format)
- Integration tests via Postman collection

## Success Criteria
- [ ] DELETE endpoint works in all three implementations
- [ ] Only "In Progress" games can be deleted
- [ ] "Won" and "Lost" games return 400 error
- [ ] Non-existent games return 404 error
- [ ] All unit tests pass
- [ ] Test coverage maintained/improved
- [ ] Postman tests updated and passing

## Notes
- Current implementations have incomplete game logic (makeGuess doesn't work properly)
- Games are stored in-memory only (lost on restart)
- No authentication/authorization is implemented
- Consider future enhancements:
  - Persistence layer
  - User authentication
  - Soft delete with audit trail