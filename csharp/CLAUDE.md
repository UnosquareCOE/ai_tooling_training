# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with the C# implementation of the Hangman API.

## Commands

```bash
dotnet restore                           # Install dependencies
dotnet run --project src/api/api.csproj  # Run application on port 4567
dotnet watch --project src/api/api.csproj # Run with hot reload
dotnet test                              # Run XUnit tests
```

## Architecture

### Project Structure
- **Solution File**: `csharp.sln` - Contains API and test projects
- **.NET Version**: 8.0
- **Framework**: ASP.NET Core Web API

### API Project (`src/api/`)
- `Program.cs` - Application entry point and service configuration
- `Controllers/GamesController.cs` - REST API endpoints
- `ViewModels/` - Data transfer objects
  - `GameViewModel.cs` - Game state representation
  - `GuessViewModel.cs` - Letter guess input model
  - `ResponseErrorViewModel.cs` - Error response structure
- `Utils/IdentifierGenerator.cs` - GUID generation utility
- `appsettings.json` - Application configuration

### Test Project (`test/api/`)
- `Controllers/GamesControllerTests.cs` - XUnit controller tests
- `Usings.cs` - Global using statements

## Key Configuration
- **Target Framework**: .NET 8.0
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled
- **Port**: 4567 (configured in launchSettings.json)

## Code Patterns
- Controller-based routing with attributes
- ViewModels for request/response DTOs
- Dependency injection via constructor
- Async/await pattern for controller actions
- Dictionary<string, GameViewModel> for in-memory storage
- GUID-based game identifiers

## Testing Patterns
- XUnit test framework
- Fact and Theory attributes for test methods
- Controller unit testing
- Arrange-Act-Assert pattern
- Test naming: MethodName_Scenario_ExpectedBehavior

## ASP.NET Core Features
- Minimal API configuration in Program.cs
- Built-in dependency injection container
- Model binding and validation
- ActionResult return types for proper HTTP responses

## Educational Notes
This implementation may contain intentional bugs or simplified patterns for learning purposes. Check the README.md for Copilot prompt examples demonstrating AI-assisted C# development, including FluentValidation integration and layered architecture patterns.