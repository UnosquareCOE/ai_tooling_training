# Hangman Game API Documentation

## Overview
This API provides endpoints for managing hangman games across multiple implementations (C#, Java, TypeScript).

## Base URLs
- **C#**: `http://localhost:4567`
- **Java**: `http://localhost:4567` 
- **TypeScript**: `http://localhost:3000`

## Endpoints

### POST /games
Creates a new hangman game.

**Response:**
- **200 OK**: Returns game ID
- **Content-Type**: `application/json`

**Example:**
```bash
curl -X POST http://localhost:4567/games
```

**Response:**
```json
"a83c2b3f-3c16-4ab2-9ea0-a4d6a2e51620"
```

### GET /games/{gameId}
Retrieves details of a specific game.

**Parameters:**
- `gameId` (path, required): UUID of the game

**Response:**
- **200 OK**: Game details
- **404 Not Found**: Game not found

**Example:**
```bash
curl -X GET http://localhost:4567/games/a83c2b3f-3c16-4ab2-9ea0-a4d6a2e51620
```

### PUT /games/{gameId}
Makes a guess in the specified game.

**Parameters:**
- `gameId` (path, required): UUID of the game

**Request Body:**
```json
{
  "letter": "a"
}
```

**Response:**
- **200 OK**: Updated game state
- **400 Bad Request**: Invalid letter format
- **404 Not Found**: Game not found

### DELETE /games/{gameId}
Deletes a game that is currently in progress.

**Parameters:**
- `gameId` (path, required): UUID of the game to delete

**Responses:**
- **204 No Content**: Game successfully deleted
- **400 Bad Request**: Invalid game ID format
- **404 Not Found**: Game not found
- **409 Conflict**: Cannot delete completed game

**Example Usage:**
```bash
# Delete a game
curl -X DELETE http://localhost:4567/games/123e4567-e89b-12d3-a456-426614174000

# Expected response: 204 No Content (empty body)
```

**Business Rules:**
- Only games with status "In Progress" can be deleted
- Games with status "Won" or "Lost" cannot be deleted
- Attempting to delete a non-existent game returns 404
- Invalid game ID format returns 400

## Error Responses

All error responses follow a consistent format:

```json
{
  "error": "Error message description"
}
```

Or for C# implementation:
```json
{
  "message": "Error message description"
}
```

## Game Status Values
- `"In Progress"`: Game is active and can be played
- `"Won"`: Player has successfully guessed the word
- `"Lost"`: Player has run out of guesses
