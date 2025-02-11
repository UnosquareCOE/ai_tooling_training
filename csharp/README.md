# C# Hangman AI Challenge

This project implements a Hangman game using the following technologies:

- C#
- .NET 8
- [ASP.NET Core](https://github.com/dotnet/aspnetcore)
- [XUnit](https://xunit.net/)

## How to run the Application

Skip the installer sections if you already have the following installed:
- .NET 8

### Manual Installation on macOS & Windows

1. **Install .NET 8**
- Visit the [dotnet website](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- Download the SDK installer relevant to your computer.
- Follow the installation wizard with the default settings.

2. **Verify Installation**
   - Open a terminal/command line.
   - Type `dotnet --version`.
   - You should see `8.0.x` or similar.

### Package Installation

You can also install .NET 8 using a package manager:
- Chocolatey: Windows
- Homebrew: macOS
- [Linux Installation Guide](https://learn.microsoft.com/en-us/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website)

## Running the Application

To run the service using the .NET runtime, follow these steps:

- Navigate to the root directory of your c# application in the terminal/command line. For this repository the command would be:
  - `cd csharp`

1. Run the following command to install the services dependencies: `dotnet restore`
2. Run the following command to start up the application: `dotnet run --project src/api/api.csproj`
  - It is also possible to `cd` into the `src/api` folder and instead run `dotnet run`.
  - `dotnet watch --project src/api/api.csproj` is often a good option when hot reloading is needed.
  - The app should be available at: `http://localhost:4567`
3. Execute the unit tests: `dotnet test`

## Prompt Examples
Below are some prompts that can be tried from the `GamesController`. These prompts are purely for examples and do not product the final result of what is expected from the challenge.

### Generate a method for Loading words from API - Ghost Text
1. Create a method to Load words
  ```
    public string[] LoadWordsFromApi()
  ```
2. Hit return and review suggestions

### Naming Suggestion and edge cases - Inline Prompt - (control/cmd & i)
1. Highlight `GetGame` method.
2. Explanation of the method.
  ```
  /explain
  ```
3. Clarity on name.
  ```
  Make this method name clearer in intent
  ```
4. Handle edge cases.
  ```
  This method should handle if a Game does not exist; it should return a NotFound response.
  ```

### Add Unit Test for MakeGuess - Inline Prompt - (control/cmd & i)
1. Highlight MakeGuess
2. Suggest tests
  ```
  /tests
  ```
3. Adjust suggested mocking framework
  ```
  Use NSubstitute instead of Moq
  ```

### Building Guess Logic - Chat Prompt - (cmd + control/control + alt & i)
1. Build out logic for MakeGuess implementation.
  ```
  MakeGuess needs to handle Checking a letter against a specific game determined by gameId. Update MakeGuess to review the letter against the game UnmaskedWord, update the game word property to unmask any correctly guessed letters and modify the RemainingGuesses account if an incorrect guess is made. If the RemainingGuesses count reaches, the Status of the game should move to "Lost" and no more guesses can be made
  ```

### Adding Fluent Validation - Chat Prompt - (cmd + control/control + alt & i)
1. Implementation and usage for FluentValidation
  ```
  Validation is currently handled inline within the GamesController, specifically the MakeGuess method. FluentValidation can provide an approach to move the existing validation for letter into a validator. Design an approach to implement FluentValidation. This approach should have the FluentValidation validator within the GuessViewModel.cs file, there should be a IValidatable inferface to return Validators, the GuessViewModel class should inherit from IValidatable, which will return the GuessValidator and lastly the GamesController.cs should be updated to use the GuessValidator returned by the GuessViewModel
  ```
2. Refinement on approach.
  ```
  Instead of calling GetValidator on the IValidatable interface within the GamesController, instead call Validate and use the validator within the implementation directly. Additionally for handling ValidationResult add a method to take in the ValidationResult which returns a BadRequest using the ResponseErrorViewModel with Message set to 'Validation Failed' and the Errors assigned to the ValidationResult Errors ErrorMessage
  ```

### Adding Layered Architectural Pattern - Chat Prompt - (cmd + control/control + alt & i)
1. Generates an example structure for a layered architecture.
  ```
  Implement a layered architecture with a presentation layer, business layer and data access layer. Presentation handling the HttpRequests within Controllers for example GamesController, business for any logic for example GamesService and data access for managing the state of the game for example GamesRepository.
  ```
