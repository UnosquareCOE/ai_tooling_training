# HE-8: Add Delete Game API

## Overview
- **Status**: In Progress
- **Assignee**: Mark Brown
- **Created**: 2025-07-23
- **Board**: 216

## Requirements
Currently within the application it is possible to create a game, retrieve a game and make a guess; however it is not possible to delete an in progress game.

This ticket covers the effort to add a DELETE API endpoint to handle the removal of in progress games. It should be invoked with a DELETE method to the endpoint `/games/{gameId}`.

**Acceptance Criteria:**
- Delete Endpoint can be invoked and remove in progress games.
- Games with a status of Won or Lost cannot be deleted.

## Technical Analysis

### Affected Components
- Game Controllers (C#, Java, TypeScript)
- Game storage (in-memory)
- Game status logic (needs implementation)
- API routing
- Unit tests

### Dependencies
- No external dependencies
- Requires game status logic to be implemented first
- In-memory storage pattern already established

## Implementation Plan

### Phase 1: Prerequisites
- [x] Implement game status updates in makeGuess endpoints
- [x] Add logic to set status to "Won" when word is fully guessed
- [x] Add logic to set status to "Lost" when remainingGuesses reaches 0
- [x] Verify status is properly persisted in game storage

### Phase 2: Core Implementation
- [x] Add DELETE route to C# GamesController
- [x] Add DELETE route to Java Spark application
- [x] Add DELETE route to TypeScript Express router
- [x] Implement consistent validation across all platforms
- [x] Return proper HTTP status codes (204, 400, 404)

### Phase 3: Testing
- [x] Write unit tests for C# implementation
- [x] Write unit tests for Java implementation
- [x] Write unit tests for TypeScript implementation
- [x] Test all edge cases and error scenarios
- [x] Verify acceptance criteria are met

## Code Locations
- `csharp/src/api/Controllers/GamesController.cs` - C# controller
- `java/app/src/main/java/hangman/App.java` - Java routes
- `typescript/routers/games.ts` - TypeScript router
- `csharp/test/api/Controllers/GamesControllerTests.cs` - C# tests
- `java/app/src/test/java/hangman/controllers/GameControllerTests.java` - Java tests
- `typescript/controllers/__tests__/games.test.ts` - TypeScript tests

## Testing Strategy

### Unit Tests
1. **Successful Deletion (204)**
   - Create game
   - Delete game with "In Progress" status
   - Verify 204 No Content response
   - Verify game is removed from storage

2. **Game Not Found (404)**
   - Attempt to delete non-existent game ID
   - Verify 404 Not Found response
   - Verify error message

3. **Cannot Delete Won Game (400)**
   - Create game with "Won" status
   - Attempt deletion
   - Verify 400 Bad Request response
   - Verify game remains in storage

4. **Cannot Delete Lost Game (400)**
   - Create game with "Lost" status
   - Attempt deletion
   - Verify 400 Bad Request response
   - Verify game remains in storage

5. **Invalid Game ID Format**
   - Test with malformed UUIDs/GUIDs
   - Verify appropriate error handling

### Integration Tests (Future)
- End-to-end API testing
- Concurrent deletion scenarios
- Performance under load

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Game status logic not implemented | High | Must implement status updates first before DELETE endpoint |
| Thread safety issues | Medium | Consider adding locks for concurrent access if needed |
| Memory leaks from undeleted games | Low | Consider implementing game expiration/TTL |
| Inconsistent behavior across platforms | Medium | Use shared test cases to ensure parity |

## Implementation Details

### C# Implementation
```csharp
[HttpDelete("{gameId:guid}")]
public ActionResult DeleteGame(Guid gameId)
{
    if (!Games.ContainsKey(gameId))
        return NotFound(new ResponseErrorViewModel { Message = "Game not found" });
    
    var game = Games[gameId];
    if (game.Status != "In Progress")
        return BadRequest(new ResponseErrorViewModel { 
            Message = "Only games with status 'In Progress' can be deleted" 
        });
    
    Games.Remove(gameId);
    return NoContent();
}
```

### Java Implementation
```java
delete("/games/:game_id", (request, response) -> {
    var gameIdString = request.params("game_id");
    var gameId = UUID.fromString(gameIdString);
    
    if (!games.containsKey(gameId)) {
        response.status(404);
        return gson.toJson(new ResponseError("Game not found"));
    }
    
    var game = games.get(gameId);
    if (!game.getStatus().equals("In Progress")) {
        response.status(400);
        return gson.toJson(new ResponseError(
            "Only games with status 'In Progress' can be deleted"
        ));
    }
    
    games.remove(gameId);
    response.status(204);
    return "";
});
```

### TypeScript Implementation
```typescript
router.delete("/:gameId", (req: Request, res: Response) => {
    const gameId = req.params.gameId;
    const game = retrieveGame(gameId);
    
    if (!game) {
        res.status(404).json({ message: "Game not found" });
        return;
    }
    
    if (game.status !== "In Progress") {
        res.status(400).json({ 
            message: "Only games with status 'In Progress' can be deleted" 
        });
        return;
    }
    
    delete games[gameId];
    res.status(204).send();
});
```

## Progress Log
- **2025-07-23**: Initial analysis complete, implementation plan created
- **2025-07-23**: Jira ticket updated with technical details
- **2025-07-23**: Local documentation created
- **2025-07-23**: Phase 1 completed - Game status logic implemented in all three platforms
- **2025-07-23**: Phase 2 completed - DELETE endpoints added to all implementations
- **2025-07-23**: Phase 3 completed - Unit tests written for all platforms
- **2025-07-23**: All acceptance criteria verified and met

## Completion Summary

### Acceptance Criteria Verification
✅ **Delete Endpoint can be invoked and remove in progress games**
- DELETE /games/{gameId} endpoint implemented in all three platforms
- Returns 204 No Content on successful deletion
- Game is removed from in-memory storage

✅ **Games with a status of Won or Lost cannot be deleted**
- Returns 400 Bad Request when attempting to delete Won/Lost games
- Error message clearly states: "Only games with status 'In Progress' can be deleted"
- Game remains in storage when deletion is rejected

### Implementation Highlights
1. **Consistent behavior across platforms**: All three implementations follow the same logic
2. **Proper HTTP status codes**: 204 for success, 400 for invalid state, 404 for not found
3. **Game status logic**: Implemented Win/Lost conditions in makeGuess endpoints
4. **Comprehensive testing**: Unit tests cover all scenarios including edge cases