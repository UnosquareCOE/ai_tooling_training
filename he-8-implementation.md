# HE-8 Implementation Plan: Add Delete Game API

## Summary of Ticket

**Ticket:** HE-8  
**Title:** Add Delete Game API  
**Status:** To Do  
**Priority:** Medium  

**Overview:** Currently within the application it is possible to create a game, retrieve a game and make a guess; however it is not possible to delete an `in progress` game.

This ticket covers the effort to add a DELETE API endpoint to handle the removal of `in progress` games. It should be invoked with a DELETE method to the endpoint `/games/{gameId}`.

**Acceptance Criteria:**
- Delete Endpoint can be invoked and remove `in progress` games.
- Games with a status of `Won` or `Lost` cannot be deleted.

## Details on Current Application State

### Architecture Overview
The project contains three implementations of a hangman game API:
1. **C# ASP.NET Core API** - Located in `/csharp/`
2. **Java Spark Framework API** - Located in `/java/`
3. **TypeScript Express API** - Located in `/typescript/`

### Current Functionality Analysis

#### Common Features Across All Implementations:
1. **POST /games** - Create a new game
2. **GET /games/{gameId}** - Retrieve a game by ID
3. **PUT /games/{gameId}** - Make a guess (incomplete implementation)

#### Current Game States:
- **In Progress** - Initial state when game is created
- **Won** - When player successfully guesses the word (not yet implemented)
- **Lost** - When player runs out of guesses (not yet implemented)

#### Current Implementation Status:
- **C#**: Basic structure in place, guess logic incomplete
- **Java**: Basic structure in place, guess logic incomplete  
- **TypeScript**: Basic structure in place, guess logic incomplete

#### Existing Test Coverage:
- **C#**: Basic test for game creation
- **Java**: Basic test for game creation
- **TypeScript**: Basic test for game creation
- **Postman**: Integration tests including DELETE endpoints (expectations already defined)

## Step-by-Step Implementation Details

### Phase 1: Complete Game Logic (Required for Delete Validation)

Before implementing delete functionality, we need to complete the game logic to properly handle `Won` and `Lost` states.

#### Game Logic Requirements:
1. Track correct and incorrect guesses
2. Update word display when correct letters are guessed
3. Decrease remaining guesses for incorrect letters
4. Set status to "Won" when word is completely guessed
5. Set status to "Lost" when remaining guesses reach 0

### Phase 2: Implement Delete Game Functionality

#### Delete Endpoint Specifications:
- **Method:** DELETE
- **Path:** `/games/{gameId}`
- **Success Response:** 204 No Content
- **Error Responses:** 
  - 404 Not Found (game doesn't exist)
  - 400 Bad Request (game status is Won or Lost)

#### Business Logic:
1. Validate game exists
2. Check game status is "In Progress"
3. Remove game from storage
4. Return appropriate HTTP status

### Phase 3: Add Comprehensive Tests

## Implementation Code and Changes Required

### C# Implementation

#### 1. Complete Game Logic in GamesController.cs

```csharp
[HttpPut("{gameId:guid}")]
public ActionResult<GameViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
{
    if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
    {
        return BadRequest(new ResponseErrorViewModel
        {
            Message = "Letter cannot accept more than 1 character"
        });
    }
    
    var game = RetrieveGame(gameId);
    if (game == null)
    {
        return NotFound(new ResponseErrorViewModel
        {
            Message = "Game not found"
        });
    }

    var letter = guessViewModel.Letter.ToLower();
    var isCorrectGuess = game.UnmaskedWord!.ToLower().Contains(letter);

    if (isCorrectGuess)
    {
        // Update word display
        var wordArray = game.Word!.ToCharArray();
        var unmaskedArray = game.UnmaskedWord!.ToCharArray();
        
        for (int i = 0; i < unmaskedArray.Length; i++)
        {
            if (unmaskedArray[i].ToString().ToLower() == letter)
            {
                wordArray[i] = unmaskedArray[i];
            }
        }
        
        game.Word = new string(wordArray);
        
        // Check if word is complete
        if (!game.Word.Contains('_'))
        {
            game.Status = "Won";
        }
    }
    else
    {
        // Add to incorrect guesses and decrease remaining
        if (!game.IncorrectGuesses.Contains(letter))
        {
            game.IncorrectGuesses.Add(letter);
            game.RemainingGuesses--;
        }
        
        // Check if game is lost
        if (game.RemainingGuesses <= 0)
        {
            game.Status = "Lost";
        }
    }

    return Ok(game);
}
```

#### 2. Add Delete Endpoint in GamesController.cs

```csharp
[HttpDelete("{gameId:guid}")]
public ActionResult DeleteGame([FromRoute] Guid gameId)
{
    var game = RetrieveGame(gameId);
    if (game == null)
    {
        return NotFound(new ResponseErrorViewModel
        {
            Message = "Game not found"
        });
    }

    if (game.Status != "In Progress")
    {
        return BadRequest(new ResponseErrorViewModel
        {
            Message = "Cannot delete games with status Won or Lost"
        });
    }

    Games.Remove(gameId);
    return NoContent();
}
```

#### 3. Update RetrieveGame Method

```csharp
private static GameViewModel? RetrieveGame(Guid gameId)
{
    return Games.GetValueOrDefault(gameId);
}
```

#### 4. Add Tests in GamesControllerTests.cs

```csharp
[Fact]
public void DeleteGame_WhenGameInProgress_ReturnsNoContent()
{
    // Arrange
    var gameId = Guid.NewGuid();
    var controller = RetrieveController(new MockIdentifierGenerator(gameId));
    controller.CreateGame();

    // Act
    var result = controller.DeleteGame(gameId);

    // Assert
    Assert.IsType<NoContentResult>(result);
}

[Fact]
public void DeleteGame_WhenGameNotFound_ReturnsNotFound()
{
    // Arrange
    var controller = RetrieveController(new MockIdentifierGenerator(Guid.NewGuid()));
    var nonExistentId = Guid.NewGuid();

    // Act
    var result = controller.DeleteGame(nonExistentId);

    // Assert
    Assert.IsType<NotFoundObjectResult>(result);
}

[Fact]
public void DeleteGame_WhenGameWon_ReturnsBadRequest()
{
    // Arrange
    var gameId = Guid.NewGuid();
    var controller = RetrieveController(new MockIdentifierGenerator(gameId));
    controller.CreateGame();
    
    // Manually set game status to Won for testing
    // This would require making Games accessible or adding a helper method

    // Act
    var result = controller.DeleteGame(gameId);

    // Assert
    Assert.IsType<BadRequestObjectResult>(result);
}
```

### Java Implementation

#### 1. Complete Game Logic in GamesController.java

```java
public Game makeGuess(Request request, Response response) {
    var game = getGame(request, response);
    if (game != null) {
        var guess = new Gson().fromJson(request.body(), Guess.class);

        if (guess == null || guess.getLetter() == null || guess.getLetter().length() != 1) {
            throw new IllegalArgumentException("Guess must be supplied with 1 letter");
        }

        String letter = guess.getLetter().toLowerCase();
        boolean isCorrectGuess = game.getUnmaskedWord().toLowerCase().contains(letter);

        if (isCorrectGuess) {
            // Update word display
            StringBuilder wordBuilder = new StringBuilder(game.getWord());
            String unmaskedWord = game.getUnmaskedWord();
            
            for (int i = 0; i < unmaskedWord.length(); i++) {
                if (String.valueOf(unmaskedWord.charAt(i)).toLowerCase().equals(letter)) {
                    wordBuilder.setCharAt(i, unmaskedWord.charAt(i));
                }
            }
            
            game.setWord(wordBuilder.toString());
            
            // Check if word is complete
            if (!game.getWord().contains("_")) {
                game.setStatus("Won");
            }
        } else {
            // Add to incorrect guesses and decrease remaining
            if (!game.getIncorrectGuesses().contains(letter)) {
                game.getIncorrectGuesses().add(letter);
                game.setRemainingGuesses(game.getRemainingGuesses() - 1);
            }
            
            // Check if game is lost
            if (game.getRemainingGuesses() <= 0) {
                game.setStatus("Lost");
            }
        }

        return game;
    }
    return null;
}
```

#### 2. Add Delete Method in GamesController.java

```java
public Object deleteGame(Request request, Response response) {
    var gameArgument = request.params("game_id");
    var gameId = UUID.fromString(gameArgument);
    
    if (gameId == null || !games.containsKey(gameId)) {
        response.status(404);
        return new ResponseError("Game not found");
    }

    var game = games.get(gameId);
    if (!game.getStatus().equals("In Progress")) {
        response.status(400);
        return new ResponseError("Cannot delete games with status Won or Lost");
    }

    games.remove(gameId);
    response.status(204);
    return "";
}
```

#### 3. Add Getters/Setters to Game.java

```java
// Add these methods to Game.java
public int getRemainingGuesses() { return remainingGuesses; }
public void setRemainingGuesses(int remainingGuesses) { this.remainingGuesses = remainingGuesses; }
public String getWord() { return word; }
public void setWord(String word) { this.word = word; }
public String getUnmaskedWord() { return unmaskedWord; }
public String getStatus() { return status; }
public void setStatus(String status) { this.status = status; }
public List<String> getIncorrectGuesses() { return incorrectGuesses; }
```

#### 4. Update App.java Routing

```java
// Add this line to App.java main method
delete("/games/:game_id", (request, response) -> gamesController.deleteGame(request, response), new JsonTransformer());
```

#### 5. Add Tests in GameControllerTests.java

```java
@Test
public void deleteGameReturnsNoContentForInProgressGame() {
    // Arrange
    var gameId = UUID.randomUUID();
    var mockIdentifierGenerator = new MockIdentifierGenerator(gameId);
    var gameController = new GamesController(mockIdentifierGenerator);
    gameController.createGame();

    // Create mock request/response
    var mockRequest = mock(Request.class);
    var mockResponse = mock(Response.class);
    when(mockRequest.params("game_id")).thenReturn(gameId.toString());

    // Act
    var result = gameController.deleteGame(mockRequest, mockResponse);

    // Assert
    verify(mockResponse).status(204);
    assertEquals("", result);
}
```

### TypeScript Implementation

#### 1. Complete Game Logic in games.ts

```typescript
function makeGuess(req: Request, res: Response) {
  const { gameId } = req.params;
  const { letter } = req.body;

  if (!letter || letter.length != 1) {
    res.status(400).json({
      message: "Letter cannot accept more than 1 character",
    });
    return;
  }

  const game = retrieveGame(gameId);
  if (!game) {
    res.status(404).json({
      message: "Game not found",
    });
    return;
  }

  const lowerLetter = letter.toLowerCase();
  const isCorrectGuess = game.unmaskedWord.toLowerCase().includes(lowerLetter);

  if (isCorrectGuess) {
    // Update word display
    let wordArray = game.word.split('');
    const unmaskedArray = game.unmaskedWord.split('');
    
    for (let i = 0; i < unmaskedArray.length; i++) {
      if (unmaskedArray[i].toLowerCase() === lowerLetter) {
        wordArray[i] = unmaskedArray[i];
      }
    }
    
    game.word = wordArray.join('');
    
    // Check if word is complete
    if (!game.word.includes('_')) {
      game.status = "Won";
    }
  } else {
    // Add to incorrect guesses and decrease remaining
    if (!game.incorrectGuesses.includes(lowerLetter)) {
      game.incorrectGuesses.push(lowerLetter);
      game.remainingGuesses--;
    }
    
    // Check if game is lost
    if (game.remainingGuesses <= 0) {
      game.status = "Lost";
    }
  }

  res.status(200).json(clearUnmaskedWord(game));
}
```

#### 2. Add Delete Function in games.ts

```typescript
function deleteGame(req: Request, res: Response) {
  const { gameId } = req.params;
  const game = retrieveGame(gameId);

  if (!game) {
    res.status(404).json({
      message: "Game not found",
    });
    return;
  }

  if (game.status !== "In Progress") {
    res.status(400).json({
      message: "Cannot delete games with status Won or Lost",
    });
    return;
  }

  delete games[gameId];
  res.status(204).send();
}
```

#### 3. Update Controller Export

```typescript
const GamesController = {
  createGame,
  getGame,
  makeGuess,
  deleteGame,
};
```

#### 4. Update Router in games.ts

```typescript
GamesRouter.route("/:gameId").delete(GamesController.deleteGame);
```

#### 5. Add Tests in games.test.ts

```typescript
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

  it("Should return 400 when trying to delete Won game", () => {
    // Setup a won game and test deletion
    const req = mockRequest({ params: { gameId: mockId } });
    const res = mockResponse();

    // First create a game
    GamesController.createGame(req, res);
    
    // Manually set status to Won (would need access to games object)
    // This test would need to be implemented with proper setup

    GamesController.deleteGame(req, res);

    expect(res.status).toHaveBeenCalledWith(400);
  });

  it("Should return 204 when deleting in-progress game", () => {
    const req = mockRequest({ params: { gameId: mockId } });
    const res = mockResponse();

    // Create game first
    GamesController.createGame(req, res);

    // Delete game
    GamesController.deleteGame(req, res);

    expect(res.status).toHaveBeenCalledWith(204);
    expect(res.send).toHaveBeenCalledWith();
  });
});
```

## Comprehensive TODO List

### C# (.NET) - `/csharp/` folder

#### Code Changes:
- [ ] **Complete guess logic in `MakeGuess` method** (GamesController.cs)
  - [ ] Implement correct guess handling (update word display)
  - [ ] Implement incorrect guess handling (add to incorrect guesses, decrement remaining)
  - [ ] Implement win condition check (no underscores remaining)
  - [ ] Implement lose condition check (remaining guesses = 0)
  - [ ] Add proper error handling for game not found

- [ ] **Add `DeleteGame` method** (GamesController.cs)
  - [ ] Add HTTP DELETE endpoint with gameId parameter
  - [ ] Validate game exists (return 404 if not found)
  - [ ] Validate game status is "In Progress" (return 400 if Won/Lost)
  - [ ] Remove game from Games dictionary
  - [ ] Return 204 No Content on success

- [ ] **Fix `RetrieveWord` method** (GamesController.cs)
  - [ ] Fix random index calculation (currently has bug with bounds)

#### Testing:
- [ ] **Add comprehensive unit tests** (GamesControllerTests.cs)
  - [ ] Test `MakeGuess` with correct letter
  - [ ] Test `MakeGuess` with incorrect letter
  - [ ] Test `MakeGuess` with invalid input
  - [ ] Test `MakeGuess` win condition
  - [ ] Test `MakeGuess` lose condition
  - [ ] Test `DeleteGame` with valid in-progress game
  - [ ] Test `DeleteGame` with non-existent game
  - [ ] Test `DeleteGame` with Won game
  - [ ] Test `DeleteGame` with Lost game

#### Build & Run:
- [ ] **Verify build and test execution**
  - [ ] Run `dotnet build` to ensure compilation
  - [ ] Run `dotnet test` to execute unit tests
  - [ ] Run `dotnet run` to start API server
  - [ ] Test endpoints with Postman collection

### Java (Spark) - `/java/` folder

#### Code Changes:
- [ ] **Complete guess logic in `makeGuess` method** (GamesController.java)
  - [ ] Implement correct guess handling
  - [ ] Implement incorrect guess handling
  - [ ] Implement win/lose condition checks
  - [ ] Add proper error handling

- [ ] **Add getters/setters to Game model** (Game.java)
  - [ ] Add `getRemainingGuesses()` and `setRemainingGuesses()`
  - [ ] Add `getWord()` and `setWord()`
  - [ ] Add `getUnmaskedWord()`
  - [ ] Add `getStatus()` and `setStatus()`
  - [ ] Add `getIncorrectGuesses()`

- [ ] **Add `deleteGame` method** (GamesController.java)
  - [ ] Implement game validation and deletion logic
  - [ ] Return appropriate HTTP status codes
  - [ ] Handle error cases (404, 400)

- [ ] **Update routing** (App.java)
  - [ ] Add DELETE route for `/games/:game_id`
  - [ ] Ensure proper JSON transformer usage

#### Testing:
- [ ] **Add comprehensive unit tests** (GameControllerTests.java)
  - [ ] Test game creation
  - [ ] Test guess functionality
  - [ ] Test delete functionality for all scenarios
  - [ ] Mock Request/Response objects properly

#### Build & Run:
- [ ] **Verify build and test execution**
  - [ ] Run `./gradlew build` to ensure compilation
  - [ ] Run `./gradlew test` to execute unit tests
  - [ ] Run `./gradlew run` to start API server
  - [ ] Test endpoints with Postman collection

### TypeScript (Express) - `/typescript/` folder

#### Code Changes:
- [ ] **Complete guess logic in `makeGuess` function** (controllers/games.ts)
  - [ ] Implement correct guess handling
  - [ ] Implement incorrect guess handling
  - [ ] Implement win/lose condition checks
  - [ ] Add proper error handling for game not found

- [ ] **Add `deleteGame` function** (controllers/games.ts)
  - [ ] Implement game validation and deletion logic
  - [ ] Return appropriate HTTP status codes
  - [ ] Handle error cases (404, 400)

- [ ] **Update controller exports** (controllers/games.ts)
  - [ ] Export `deleteGame` function

- [ ] **Update routing** (routers/games.ts)
  - [ ] Add DELETE route for `/:gameId`

- [ ] **Fix word selection logic** (controllers/games.ts)
  - [ ] Fix `retrieveWord` function random selection

#### Testing:
- [ ] **Add comprehensive unit tests** (controllers/__tests__/games.test.ts)
  - [ ] Test `makeGuess` functionality
  - [ ] Test `deleteGame` for all scenarios
  - [ ] Improve mock setup for better test coverage
  - [ ] Add tests for win/lose conditions

#### Build & Run:
- [ ] **Verify build and test execution**
  - [ ] Run `npm install` to install dependencies
  - [ ] Run `npm test` to execute unit tests
  - [ ] Run `npm start` to start API server
  - [ ] Test endpoints with Postman collection

### Integration Testing - `/tests/` folder

#### Postman Testing:
- [ ] **Update Postman collection** (postman.api.json)
  - [ ] Verify existing DELETE tests are correct
  - [ ] Add test for deleting Won game (should return 400)
  - [ ] Add test for deleting Lost game (should return 400)
  - [ ] Ensure test sequence creates appropriate game states

- [ ] **Execute full integration tests**
  - [ ] Test against C# API (typically port 5000)
  - [ ] Test against Java API (typically port 4567)
  - [ ] Test against TypeScript API (typically port 3000)
  - [ ] Verify all endpoints return expected responses

### Documentation Updates

- [ ] **Update README files**
  - [ ] Update main README.md with delete endpoint documentation
  - [ ] Update individual language README files
  - [ ] Document API endpoints and expected responses
  - [ ] Include examples of delete operations

- [ ] **API Documentation**
  - [ ] Document delete endpoint specification
  - [ ] Include error response examples
  - [ ] Document business rules for deletion

## Implementation Priority

1. **Phase 1**: Complete the guess logic in all three implementations
2. **Phase 2**: Implement delete functionality in all three implementations  
3. **Phase 3**: Add comprehensive unit tests
4. **Phase 4**: Execute integration tests and verify Postman collection
5. **Phase 5**: Update documentation

## Success Criteria

- [ ] All Postman tests pass for all three API implementations
- [ ] Unit test coverage includes all new functionality
- [ ] Delete endpoint properly validates game status
- [ ] Error responses follow consistent format across implementations
- [ ] Games with "Won" or "Lost" status cannot be deleted
- [ ] Only "In Progress" games can be successfully deleted
- [ ] Appropriate HTTP status codes returned for all scenarios
