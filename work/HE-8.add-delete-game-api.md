# Implementation Plan: HE-8 - Add Delete Game API

## 1. Executive Summary
- **Objective**: Add a DELETE endpoint to remove in-progress Hangman games across TypeScript, C#, and Java implementations
- **Scope**: New API endpoint implementation with business rule validation, maintaining consistency across all three language implementations
- **Estimated Effort**: 4-6 hours total
  - Development: 3 hours (1 hour per language)
  - Testing: 2 hours
  - Documentation: 1 hour
- **Priority**: Medium (from ticket)
- **Risk Level**: Low - Adding new functionality without modifying existing endpoints

## 2. Technical Approach

### Architecture Decision
- **Pattern**: RESTful DELETE operation following existing controller patterns
- **Storage**: Direct removal from in-memory storage (no soft delete)
- **Response Format**: Follow REST conventions - 204 No Content for successful deletion
- **Validation**: Pre-deletion status check to enforce business rules

### Implementation Strategy
1. Add DELETE route mapping in each framework's router
2. Implement controller method with status validation
3. Return appropriate HTTP status codes based on validation results
4. Add comprehensive unit tests for all scenarios

## 3. Detailed Implementation Steps

### 3.1 TypeScript Implementation
1. **Add DELETE route** in `/typescript/routers/games.ts`
   - Map `DELETE /games/:gameId` to controller method
   
2. **Implement deleteGame controller** in `/typescript/controllers/games.ts`
   - Retrieve game from storage by ID
   - Return 404 if game not found
   - Validate game status is "In Progress"
   - Return 400 if status is "Won" or "Lost"
   - Remove game from storage
   - Return 204 No Content

3. **Add unit tests** in `/typescript/controllers/__tests__/games.test.ts`
   - Test successful deletion
   - Test 404 for non-existent game
   - Test 400 for completed games

### 3.2 C# Implementation
1. **Add DELETE action** in `/csharp/src/api/Controllers/GamesController.cs`
   ```csharp
   [HttpDelete("{gameId:guid}")]
   public IActionResult DeleteGame(Guid gameId)
   ```
   
2. **Implement deletion logic**
   - Check if game exists in _games dictionary
   - Validate status is "In Progress"
   - Remove from dictionary
   - Return appropriate status code

3. **Add unit tests** in `/csharp/test/api/Controllers/GamesControllerTests.cs`
   - Test all scenarios with proper assertions

### 3.3 Java Implementation
1. **Add DELETE route** in `/java/app/src/main/java/hangman/App.java`
   ```java
   delete("/games/:gameId", controller::deleteGame);
   ```
   
2. **Implement deleteGame method** in `/java/app/src/main/java/hangman/controllers/GamesController.java`
   - Parse UUID from gameId parameter
   - Retrieve and validate game
   - Remove from games HashMap
   - Return proper response

3. **Add unit tests** in `/java/app/src/test/java/hangman/controllers/GameControllerTests.java`
   - Cover all test scenarios

## 4. Files and Components

### Files to Modify
- `/typescript/routers/games.ts`: Add DELETE route mapping
- `/typescript/controllers/games.ts`: Add deleteGame function
- `/typescript/controllers/__tests__/games.test.ts`: Add DELETE tests
- `/csharp/src/api/Controllers/GamesController.cs`: Add DeleteGame action
- `/csharp/test/api/Controllers/GamesControllerTests.cs`: Add DELETE tests
- `/java/app/src/main/java/hangman/App.java`: Add DELETE route
- `/java/app/src/main/java/hangman/controllers/GamesController.java`: Add deleteGame method
- `/java/app/src/test/java/hangman/controllers/GameControllerTests.java`: Add DELETE tests

### Files to Create
- None - all modifications to existing files

### Dependencies
- No new external dependencies required
- Uses existing framework capabilities for DELETE operations

## 5. Testing Strategy

### Unit Tests
#### Test Case 1: Successful Deletion
- **Given**: Game exists with status "In Progress"
- **When**: DELETE /games/{gameId}
- **Then**: Returns 204, game removed from storage

#### Test Case 2: Game Not Found
- **Given**: Non-existent gameId
- **When**: DELETE /games/{gameId}
- **Then**: Returns 404 with error message

#### Test Case 3: Cannot Delete Won Game
- **Given**: Game exists with status "Won"
- **When**: DELETE /games/{gameId}
- **Then**: Returns 400 with error message

#### Test Case 4: Cannot Delete Lost Game
- **Given**: Game exists with status "Lost"
- **When**: DELETE /games/{gameId}
- **Then**: Returns 400 with error message

#### Test Case 5: Invalid ID Format (C#/Java)
- **Given**: Malformed GUID/UUID
- **When**: DELETE /games/invalid-id
- **Then**: Returns 400 or 404

### Integration Tests
- Run Postman collection to verify endpoint behavior
- Test DELETE after creating a game via POST
- Verify game is actually removed (GET returns 404 after DELETE)

### Manual Testing
1. Start each server implementation
2. Create a new game via POST
3. Attempt to delete the game - should succeed
4. Verify game no longer exists via GET
5. Create and complete a game (win/lose)
6. Attempt to delete completed game - should fail with 400

## 6. Acceptance Criteria
- [x] DELETE endpoint can be invoked at /games/{gameId}
- [x] Successfully removes games with "In Progress" status
- [x] Returns 204 No Content on successful deletion
- [x] Returns 404 for non-existent games
- [x] Returns 400 when attempting to delete "Won" games
- [x] Returns 400 when attempting to delete "Lost" games
- [x] All three implementations behave consistently
- [x] Unit tests cover all scenarios
- [x] Postman tests pass for DELETE endpoint

## 7. Risk Mitigation

| Risk | Impact | Likelihood | Mitigation Strategy |
|------|--------|------------|-------------------|
| Inconsistent error messages across implementations | Low | Medium | Use standardized error message format |
| Missing status validation | Medium | Low | Implement explicit status check before deletion |
| Race condition in concurrent deletions | Low | Low | Accept for in-memory storage; would need locking in production |
| Breaking existing API contract | High | Very Low | New endpoint doesn't affect existing ones |

## 8. References

### Related Tickets
- No direct dependencies identified

### Documentation
- Postman Collection: `/tests/postman.api.json`
- Language-specific READMEs in each implementation folder

### Code Examples
- Existing GET endpoint pattern for 404 handling
- Existing PUT endpoint pattern for validation errors
- Status checking logic in game update methods

## Implementation Notes

### Consistent Error Messages
Use these exact messages across all implementations:
- Not found: `"Game not found"`
- Cannot delete: `"Can only delete games with status 'In Progress'"`

### Response Headers
Ensure 204 response includes no body (some frameworks default to empty JSON)

### Logging Considerations
Add appropriate logging for delete operations in production-ready code

---
*Generated: 2025-08-14*
*Ticket: HE-8 - Add Delete Game API*