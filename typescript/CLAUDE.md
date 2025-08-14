# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with the TypeScript implementation of the Hangman API.

## Commands

```bash
npm install          # Install dependencies
npm start            # Start development server with nodemon (watches for changes)
npm test             # Run Jest unit tests
```

## Architecture

### Entry Point
- `index.ts` - Express server setup with middleware (CORS, Morgan logging, JSON parsing)

### Routing
- `routers/games.ts` - Express router defining REST endpoints

### Controllers
- `controllers/games.ts` - Request handlers implementing game logic
- `controllers/index.ts` - Controller exports

### Testing
- `controllers/__tests__/games.test.ts` - Jest unit tests for game controller
- `jest.config.js` - Jest configuration with ts-jest preset

## Key Dependencies
- **Express 4.x** - Web framework
- **UUID** - Generate unique game IDs
- **Morgan** - HTTP request logging
- **CORS** - Cross-origin resource sharing
- **Nodemon** - Auto-restart on file changes (dev)
- **Jest + ts-jest** - Testing framework with TypeScript support

## TypeScript Configuration
- Target: ES2020 or later
- Module: CommonJS
- Strict mode enabled
- Source maps for debugging

## Code Patterns
- Async/await for asynchronous operations
- Express middleware for validation and error handling
- In-memory Map/Object for game storage
- Request/Response typing with Express types

## Testing Patterns
- Unit tests focus on controller logic
- Mock request/response objects for Express testing
- Test coverage for all game states (create, get, guess, delete)

## Educational Notes
This implementation may contain intentional bugs or simplified patterns for learning purposes. Check the README.md for Copilot prompt examples demonstrating various AI-assisted coding techniques.