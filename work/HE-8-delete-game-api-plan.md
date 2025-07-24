# HE-8: Add Delete Game API

## Executive Summary
This ticket implements a DELETE endpoint to remove in-progress games from the Hangman API across three technology stacks (C#/.NET, Java/Spark, TypeScript/Express). The endpoint validates game status before deletion to ensure only active games can be removed.

## Technical Design

### Overview
The DELETE endpoint follows RESTful principles and maintains consistency across all three implementations. The endpoint accepts a game ID as a path parameter and validates both the existence of the game and its current status before performing the deletion.

### Implementation Details

#### Endpoint Specification
- **Method**: DELETE
- **Path**: `/games/{gameId}`
- **Path Parameters**: 
  - `gameId` - UUID/GUID format game identifier
- **Response Codes**:
  - `204 No Content` - Successful deletion
  - `404 Not Found` - Game does not exist
  - `400 Bad Request` - Game is not in "In Progress" status

#### Business Rules
1. Only games with status "In Progress" can be deleted
2. Games with status "Won" or "Lost" must return 400 error
3. Non-existent games must return 404 error
4. Successful deletion returns no content (204)

### API Contract
```yaml
DELETE /games/{gameId}
Parameters:
  - name: gameId
    in: path
    required: true
    type: string
    format: uuid
Responses:
  204:
    description: Game successfully deleted
  404:
    description: Game not found
    schema:
      type: object
      properties:
        message:
          type: string
          example: "Game not found"
  400:
    description: Game cannot be deleted
    schema:
      type: object
      properties:
        message:
          type: string
          example: "Only games with status 'In Progress' can be deleted"
```

### Data Model Changes
No changes to the data model are required. The existing Game model with status field is sufficient.

## Implementation Plan

### Phase 1: Verification of Existing Implementation
- [x] Verify DELETE endpoint exists in C# implementation
- [x] Verify DELETE endpoint exists in Java implementation  
- [x] Verify DELETE endpoint exists in TypeScript implementation
- [x] Review existing test coverage
- [x] Check Postman collection for DELETE tests

### Phase 2: Code Review and Enhancement
- [ ] Review C# GamesController.DeleteGame method
- [ ] Review Java GamesController.deleteGame method
- [ ] Review TypeScript games.deleteGame function
- [ ] Ensure consistent error messages across implementations
- [ ] Verify proper HTTP status codes

### Phase 3: Testing Strategy Enhancement
- [ ] Ensure unit tests cover all scenarios:
  - [ ] Successful deletion (204)
  - [ ] Game not found (404)
  - [ ] Invalid UUID format (400)
  - [ ] Game with "Won" status (400)
  - [ ] Game with "Lost" status (400)
- [ ] Add DELETE tests to Postman collection if missing
- [ ] Verify tests pass in CI/CD pipeline

### Phase 4: Documentation Updates
- [ ] Update API documentation in README files
- [ ] Add curl examples for DELETE endpoint
- [ ] Document business rules clearly
- [ ] Update Postman collection documentation

## Testing Strategy

### Unit Tests
Each implementation should have the following test cases:

1. **Delete In-Progress Game**
   - Given: A game exists with status "In Progress"
   - When: DELETE request is made
   - Then: Return 204 No Content and remove game from storage

2. **Delete Non-Existent Game**
   - Given: Game ID does not exist
   - When: DELETE request is made
   - Then: Return 404 Not Found with error message

3. **Delete Won Game**
   - Given: A game exists with status "Won"
   - When: DELETE request is made
   - Then: Return 400 Bad Request with appropriate message

4. **Delete Lost Game**
   - Given: A game exists with status "Lost"
   - When: DELETE request is made
   - Then: Return 400 Bad Request with appropriate message

5. **Invalid Game ID Format**
   - Given: Invalid UUID/GUID format
   - When: DELETE request is made
   - Then: Return 400 Bad Request

### Integration Tests
- Use Postman collection to test against running applications
- Verify consistent behavior across all three implementations
- Test edge cases and error scenarios

#### Postman Collection Tests (`/tests/postman.api.json`)
```json
{
  "name": "Delete Valid Game",
  "event": [
    {
      "listen": "test",
      "script": {
        "exec": [
          "pm.test(\"Delete Valid Game\", function () {",
          "    pm.response.to.have.status(204);",
          "});"
        ],
        "type": "text/javascript"
      }
    }
  ],
  "request": {
    "method": "DELETE",
    "header": [],
    "url": {
      "raw": "http://localhost:4567/games/{{gameID}}",
      "protocol": "http",
      "host": ["localhost"],
      "port": "4567",
      "path": ["games", "{{gameID}}"]
    }
  }
},
{
  "name": "Delete Invalid Game",
  "event": [
    {
      "listen": "test",
      "script": {
        "exec": [
          "pm.test(\"Delete Invalid Game\", function () {",
          "    pm.response.to.have.status(404);",
          "});"
        ],
        "type": "text/javascript"
      }
    }
  ],
  "request": {
    "method": "DELETE",
    "header": [],
    "url": {
      "raw": "http://localhost:4567/games/00000000-0000-0000-0000-000000000000",
      "protocol": "http",
      "host": ["localhost"],
      "port": "4567",
      "path": ["games", "00000000-0000-0000-0000-000000000000"]
    }
  }
}
```

**Note**: The Postman collection should be enhanced with additional tests for:
- Deleting a game with "Won" status (expect 400)
- Deleting a game with "Lost" status (expect 400)
- Invalid UUID format test

### E2E Tests
- Create game → Play game → Delete in-progress game
- Create game → Win game → Attempt delete (should fail)
- Create game → Lose game → Attempt delete (should fail)

## Security Considerations
1. **Input Validation**: Validate UUID format to prevent injection attacks
2. **Error Messages**: Ensure error messages don't leak sensitive information
3. **Rate Limiting**: Consider implementing rate limiting to prevent abuse
4. **Logging**: Log deletion attempts for audit purposes
5. **Authorization**: Future enhancement - add user authentication/authorization

## Performance Considerations
1. **O(1) Lookup**: Current in-memory storage provides constant-time lookups
2. **Memory Management**: Deletion frees memory immediately
3. **No Database Impact**: In-memory implementation has no DB overhead
4. **Concurrent Access**: Consider thread-safety for production use

## Rollback Plan
1. **Version Control**: All changes tracked in Git
2. **Feature Branch**: Work isolated in feature/HE-8-delete-game-api-impl
3. **Rollback Steps**:
   - Revert commits if issues found
   - Remove DELETE route registrations
   - Remove controller methods
   - Remove unit tests
4. **Testing**: Ensure remaining endpoints still function correctly

## Review Checklist
- [ ] Code follows language-specific conventions
- [ ] All unit tests pass
- [ ] Postman tests pass
- [ ] Documentation is updated
- [ ] Error handling is consistent
- [ ] HTTP status codes are correct
- [ ] No security vulnerabilities introduced
- [ ] Performance impact assessed
- [ ] Code is properly commented
- [ ] PR description links to Jira ticket

## Implementation Code

### C# Implementation

#### Controller Method (`/csharp/src/api/Controllers/GamesController.cs`)
```csharp
[HttpDelete("{gameId:guid}")]
public ActionResult DeleteGame([FromRoute] Guid gameId)
{
    var game = RetrieveGame(gameId);
    
    if (game == null)
    {
        return NotFound();
    }

    if (game.Status != "In Progress")
    {
        return BadRequest(new ResponseErrorViewModel
        {
            Message = "Only games with status 'In Progress' can be deleted"
        });
    }

    Games.Remove(gameId);
    return NoContent();
}
```

#### Unit Tests (`/csharp/test/api/Controllers/GamesControllerTests.cs`)
```csharp
[Fact]
public void DeleteGame_WithInProgressGame_ReturnsNoContent()
{
    // Arrange
    var identifierGenerator = new MockIdentifierGenerator();
    var controller = new GamesController(identifierGenerator);
    var createResult = controller.CreateGame();
    var createActionResult = Assert.IsType<OkObjectResult>(createResult);
    var gameViewModel = Assert.IsType<GameIdViewModel>(createActionResult.Value);

    // Act
    var deleteResult = controller.DeleteGame(gameViewModel.GameId);

    // Assert
    Assert.IsType<NoContentResult>(deleteResult);
    
    // Verify game is actually deleted
    var getResult = controller.GetGame(gameViewModel.GameId);
    Assert.IsType<NotFoundResult>(getResult);
}

[Fact]
public void DeleteGame_WithNonExistentGame_ReturnsNotFound()
{
    // Arrange
    var identifierGenerator = new MockIdentifierGenerator();
    var controller = new GamesController(identifierGenerator);
    var nonExistentGameId = Guid.NewGuid();

    // Act
    var result = controller.DeleteGame(nonExistentGameId);

    // Assert
    Assert.IsType<NotFoundResult>(result);
}

[Fact]
public void DeleteGame_WithWonGame_ReturnsBadRequest()
{
    // Arrange
    var identifierGenerator = new MockIdentifierGenerator();
    var controller = new GamesController(identifierGenerator);
    var createResult = controller.CreateGame();
    var createActionResult = Assert.IsType<OkObjectResult>(createResult);
    var gameViewModel = Assert.IsType<GameIdViewModel>(createActionResult.Value);

    // Get the private static Games field using reflection
    var gamesField = typeof(GamesController).GetField("Games", 
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
    var games = (Dictionary<Guid, GameViewModel>)gamesField.GetValue(null);
    
    // Update game status to Won
    games[gameViewModel.GameId].Status = "Won";

    // Act
    var result = controller.DeleteGame(gameViewModel.GameId);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errorResponse = Assert.IsType<ResponseErrorViewModel>(badRequestResult.Value);
    Assert.Equal("Only games with status 'In Progress' can be deleted", errorResponse.Message);
}

[Fact]
public void DeleteGame_WithLostGame_ReturnsBadRequest()
{
    // Arrange
    var identifierGenerator = new MockIdentifierGenerator();
    var controller = new GamesController(identifierGenerator);
    var createResult = controller.CreateGame();
    var createActionResult = Assert.IsType<OkObjectResult>(createResult);
    var gameViewModel = Assert.IsType<GameIdViewModel>(createActionResult.Value);

    // Get the private static Games field using reflection
    var gamesField = typeof(GamesController).GetField("Games", 
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
    var games = (Dictionary<Guid, GameViewModel>)gamesField.GetValue(null);
    
    // Update game status to Lost
    games[gameViewModel.GameId].Status = "Lost";

    // Act
    var result = controller.DeleteGame(gameViewModel.GameId);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    var errorResponse = Assert.IsType<ResponseErrorViewModel>(badRequestResult.Value);
    Assert.Equal("Only games with status 'In Progress' can be deleted", errorResponse.Message);
}
```

### Java Implementation

#### Controller Method (`/java/app/src/main/java/hangman/controllers/GamesController.java`)
```java
public String deleteGame(Request request, Response response) {
    var gameArgument = request.params("game_id");
    var gameId = UUID.fromString(gameArgument);
    
    if (gameId == null || !games.containsKey(gameId)) {
        response.status(404);
        return "";
    }

    var game = games.get(gameId);
    if (!game.getStatus().equals("In Progress")) {
        response.status(400);
        throw new IllegalArgumentException("Only games with status 'In Progress' can be deleted");
    }

    games.remove(gameId);
    response.status(204);
    return "";
}
```

#### Route Registration (`/java/app/src/main/java/hangman/App.java`)
```java
delete("/games/:game_id", (request, response) -> gamesController.deleteGame(request, response));
```

#### Unit Tests (`/java/app/src/test/java/hangman/controllers/GameControllerTests.java`)
```java
@Test
void deleteGame_WithInProgressGame_ReturnsEmptyStringAnd204() {
    // Arrange
    var game = controller.createGame();
    var gameId = gson.fromJson(game, GameId.class).getGameId();
    
    when(request.params("game_id")).thenReturn(gameId.toString());
    
    // Act
    var result = controller.deleteGame(request, response);
    
    // Assert
    assertEquals("", result);
    verify(response).status(204);
    
    // Verify game is actually deleted by trying to get it
    when(request.params("game_id")).thenReturn(gameId.toString());
    controller.getGame(request, response);
    verify(response).status(404);
}

@Test
void deleteGame_WithNonExistentGame_Returns404() {
    // Arrange
    var nonExistentGameId = UUID.randomUUID();
    when(request.params("game_id")).thenReturn(nonExistentGameId.toString());
    
    // Act
    var result = controller.deleteGame(request, response);
    
    // Assert
    assertEquals("", result);
    verify(response).status(404);
}

@Test
void deleteGame_WithWonGame_Throws400Exception() {
    // Arrange
    var game = controller.createGame();
    var gameId = gson.fromJson(game, GameId.class).getGameId();
    
    // Use reflection to modify game status
    try {
        var gamesField = GamesController.class.getDeclaredField("games");
        gamesField.setAccessible(true);
        var games = (HashMap<UUID, Game>) gamesField.get(controller);
        games.get(gameId).setStatus("Won");
    } catch (Exception e) {
        fail("Failed to setup test: " + e.getMessage());
    }
    
    when(request.params("game_id")).thenReturn(gameId.toString());
    
    // Act & Assert
    var exception = assertThrows(IllegalArgumentException.class, () -> {
        controller.deleteGame(request, response);
    });
    
    assertEquals("Only games with status 'In Progress' can be deleted", exception.getMessage());
    verify(response).status(400);
}

@Test
void deleteGame_WithLostGame_Throws400Exception() {
    // Arrange
    var game = controller.createGame();
    var gameId = gson.fromJson(game, GameId.class).getGameId();
    
    // Use reflection to modify game status
    try {
        var gamesField = GamesController.class.getDeclaredField("games");
        gamesField.setAccessible(true);
        var games = (HashMap<UUID, Game>) gamesField.get(controller);
        games.get(gameId).setStatus("Lost");
    } catch (Exception e) {
        fail("Failed to setup test: " + e.getMessage());
    }
    
    when(request.params("game_id")).thenReturn(gameId.toString());
    
    // Act & Assert
    var exception = assertThrows(IllegalArgumentException.class, () -> {
        controller.deleteGame(request, response);
    });
    
    assertEquals("Only games with status 'In Progress' can be deleted", exception.getMessage());
    verify(response).status(400);
}

@Test
void deleteGame_WithInvalidUUID_ThrowsException() {
    // Arrange
    when(request.params("game_id")).thenReturn("invalid-uuid");
    
    // Act & Assert
    assertThrows(IllegalArgumentException.class, () -> {
        controller.deleteGame(request, response);
    });
}
```

### TypeScript Implementation

#### Controller Function (`/typescript/controllers/games.ts`)
```typescript
function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;
  const game = retrieveGame(gameId);

  if (!game) {
    res.status(404).send();
    return;
  }

  if (game.status !== "In Progress") {
    res.status(400).json({
      message: "Only games with status 'In Progress' can be deleted",
    });
    return;
  }

  delete games[gameId];
  res.status(204).send();
}
```

#### Route Registration (`/typescript/routers/games.ts`)
```typescript
GamesRouter.route("/:gameId").delete(GamesController.deleteGame);
```

#### Unit Tests (`/typescript/controllers/__tests__/games.test.ts`)
```typescript
describe("deleteGame", () => {
  it("Should delete an in-progress game and return 204", () => {
    // Arrange
    const createReq = {
      body: {},
    } as Request;
    GamesController.createGame(createReq, res);
    
    const gameId = mockUuid;
    req.params = { gameId };

    // Act
    GamesController.deleteGame(req, res);

    // Assert
    expect(res.status).toHaveBeenCalledWith(204);
    expect(res.send).toHaveBeenCalled();
    
    // Verify game is actually deleted
    GamesController.getGame(req, res);
    expect(res.status).toHaveBeenCalledWith(404);
  });

  it("Should return 404 for non-existent game", () => {
    // Arrange
    req.params = { gameId: "non-existent-id" };

    // Act
    GamesController.deleteGame(req, res);

    // Assert
    expect(res.status).toHaveBeenCalledWith(404);
    expect(res.send).toHaveBeenCalled();
  });

  it("Should return 400 when trying to delete a won game", () => {
    // Arrange
    const createReq = { body: {} } as Request;
    GamesController.createGame(createReq, res);
    
    const gameId = mockUuid;
    games[gameId].status = "Won";
    req.params = { gameId };

    // Act
    GamesController.deleteGame(req, res);

    // Assert
    expect(res.status).toHaveBeenCalledWith(400);
    expect(res.json).toHaveBeenCalledWith({
      message: "Only games with status 'In Progress' can be deleted",
    });
  });

  it("Should return 400 when trying to delete a lost game", () => {
    // Arrange
    const createReq = { body: {} } as Request;
    GamesController.createGame(createReq, res);
    
    const gameId = mockUuid;
    games[gameId].status = "Lost";
    req.params = { gameId };

    // Act
    GamesController.deleteGame(req, res);

    // Assert
    expect(res.status).toHaveBeenCalledWith(400);
    expect(res.json).toHaveBeenCalledWith({
      message: "Only games with status 'In Progress' can be deleted",
    });
  });
});
```

## Implementation Notes

### Current Status
Based on code analysis, DELETE endpoints have been previously implemented in all three codebases with commit hash `44f1600`. All implementations follow the acceptance criteria correctly with proper status validation and error handling:

- **C#**: Uses ASP.NET Core attribute routing with strong typing
- **Java**: Uses Spark framework with manual route registration
- **TypeScript**: Uses Express.js with separate router configuration

### Key Implementation Details
1. All implementations use in-memory storage (Dictionary/HashMap/Object)
2. Consistent error messages across all platforms
3. Proper HTTP status codes (204, 400, 404)
4. Comprehensive test coverage including edge cases
5. Tests use reflection (C#/Java) or direct access (TypeScript) to modify game state

### Example Usage

#### Create and Delete a Game
```bash
# Create a new game
curl -X POST http://localhost:4567/games
# Response: {"gameId":"550e8400-e29b-41d4-a716-446655440000"}

# Delete the game
curl -X DELETE http://localhost:4567/games/550e8400-e29b-41d4-a716-446655440000
# Response: 204 No Content

# Try to get the deleted game
curl -X GET http://localhost:4567/games/550e8400-e29b-41d4-a716-446655440000
# Response: 404 Not Found
```

#### Error Scenarios
```bash
# Try to delete a non-existent game
curl -X DELETE http://localhost:4567/games/00000000-0000-0000-0000-000000000000
# Response: 404 Not Found

# Try to delete a won game (would need to win a game first)
curl -X DELETE http://localhost:4567/games/{won-game-id}
# Response: 400 Bad Request
# Body: {"message":"Only games with status 'In Progress' can be deleted"}

# Try to delete with invalid UUID
curl -X DELETE http://localhost:4567/games/invalid-uuid
# Response: 400 Bad Request (varies by implementation)
```