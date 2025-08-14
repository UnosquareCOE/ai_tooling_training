# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is an educational Hangman game API project designed to teach developers effective AI tooling usage (GitHub Copilot, Claude, etc.). The repository contains three complete implementations of the same RESTful API in TypeScript, C#, and Java.

## Language-Specific Guidance

Each implementation has its own CLAUDE.md file with detailed instructions:
- **TypeScript**: See `/typescript/CLAUDE.md`
- **Java**: See `/java/CLAUDE.md`  
- **C#**: See `/csharp/CLAUDE.md`

## Quick Start Commands

### TypeScript
```bash
cd typescript && npm install && npm start  # Port 4567
```

### C#
```bash
cd csharp && dotnet run --project src/api/api.csproj  # Port 4567
```

### Java
```bash
cd java && gradle run  # Port 4567 (or ./gradlew run)
```

### API Testing
```bash
newman run tests/postman.api.json  # Test any running implementation
```

## Shared API Contract

All implementations expose identical REST endpoints:
- `POST /games` - Create new game (returns gameId)
- `GET /games/{gameId}` - Get game status
- `PUT /games/{gameId}` - Make a letter guess (body: `{"letter": "a"}`)
- `DELETE /games/{gameId}` - Delete game

### Response Format
```json
{
  "gameId": "uuid",
  "word": "_ _ _ _ _ _",
  "guessedLetters": ["a", "e"],
  "incorrectGuesses": ["e"],
  "attemptsRemaining": 4,
  "status": "In Progress | Won | Lost"
}
```

## Common Configuration
- **Port**: 4567 (all implementations)
- **Word List**: ["banana", "canine", "unosquare", "airport"]
- **Storage**: In-memory (no database)
- **Max Attempts**: Varies by implementation (3-5) - intentional for educational purposes

## CI/CD Workflows

GitHub Actions workflows in `.github/workflows/`:
- `verify-typescript.yaml`, `verify-typescript-api.yaml` - TypeScript tests
- `verify-csharp-unit.yaml`, `verify-csharp-api.yaml` - C# tests
- `verify-java-unit.yaml`, `verify-java-api.yaml` - Java tests
- `verify-agent.yaml` - PR-Agent with OpenAI integration

## Educational Context

This repository contains intentional bugs and variations for learning purposes:
1. Check language-specific README files for Copilot prompt examples
2. Review the Postman collection (`tests/postman.api.json`) for expected behavior
3. Differences between implementations are deliberate teaching points