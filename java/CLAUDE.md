# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with the Java implementation of the Hangman API.

## Commands

```bash
gradle dependencies  # Install dependencies (or ./gradlew dependencies)
gradle run           # Start application on port 4567 (or ./gradlew run)
gradle test -i       # Run JUnit tests with info logging (or ./gradlew test -i)
```

## Architecture

### Project Structure
- **Gradle Build**: Kotlin DSL (`build.gradle.kts`)
- **Java Version**: 21 (specified in toolchain)
- **Main Class**: `hangman.App`

### Package Organization (`app/src/main/java/hangman/`)
- `App.java` - Spark server setup and route definitions
- `controllers/GamesController.java` - REST endpoint handlers
- `models/` - Data models (Game, Guess, ResponseError)
- `utils/IdentifierGenerator.java` - UUID generation utility
- `interfaces/IdentifierGeneration.java` - Interface for ID generation
- `transformers/JsonTransformer.java` - Gson JSON serialization

### Testing (`app/src/test/java/hangman/`)
- `controllers/GameControllerTests.java` - JUnit 5 tests
- `mocks/MockIdentifierGenerator.java` - Test mock for ID generation

## Key Dependencies
- **Spark Java 2.9.4** - Micro web framework
- **Gson 2.10.1** - JSON serialization/deserialization
- **Log4j2 2.24.3** - Logging framework with SLF4J bridge
- **JUnit Jupiter** - JUnit 5 testing framework
- **Guava** - Google core libraries

## Code Patterns
- Spark route definitions with lambda expressions
- Gson for JSON transformation
- HashMap for in-memory game storage
- Interface-based design for testability (IdentifierGeneration)
- Static methods in controller for route handlers

## Testing Patterns
- JUnit 5 (Jupiter) with annotations (@Test, @BeforeEach)
- Mock implementations for external dependencies
- Assertions using JUnit assertions API
- Test naming convention following Java standards

## Gradle Wrapper
The project includes Gradle wrapper (`gradlew` and `gradlew.bat`) so Gradle doesn't need to be installed separately. Use `./gradlew` on Unix-like systems or `gradlew.bat` on Windows.

## Educational Notes
This implementation may contain intentional bugs (e.g., incorrect max attempts) for learning purposes. Check the README.md for Copilot prompt examples demonstrating AI-assisted Java development patterns.