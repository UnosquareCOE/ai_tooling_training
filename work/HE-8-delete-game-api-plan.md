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

## Implementation Notes

### Current Status
Based on code analysis, DELETE endpoints have been previously implemented in all three codebases with commit hash `44f1600`. The implementations include:

**C# Implementation**
- Method: `GamesController.DeleteGame(Guid gameId)`
- Route: `[HttpDelete("{gameId:guid}")]`
- Full test coverage in `GamesControllerTests.cs`

**Java Implementation**
- Method: `GamesController.deleteGame(Request request, Response response)`
- Route: `delete("/games/:game_id", ...)`
- Test coverage in `GameControllerTests.java`

**TypeScript Implementation**
- Function: `deleteGame(req: Request, res: Response)`
- Route: `GamesRouter.route("/:gameId").delete(...)`
- Test coverage in `games.test.ts`

All implementations follow the acceptance criteria correctly with proper status validation and error handling.