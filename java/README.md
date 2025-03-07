# Java Hangman AI Challenge

This project implements a Hangman game using the following technologies:

- Java 21
- [Spark](http://sparkjava.com/)
  - Spark is a micro web framework that is used to expose API endpoints.
- [Gson](https://github.com/google/gson)
  - GSON is a framework from google that we use to convert to & from JSON.
- JUnit version 4

## How to run the Application

Skip the installer sections if you already have the following installed:

- Java 21

### Manual Installation on macOS & Windows

1. **Install Sdkman & Java 21**

- Open a terminal/command line.
- Copy command `curl -s "https://get.sdkman.io" | bash` and run in terminal
  - See [Sdkman Website](https://sdkman.io/)
- Run `sdk install java` in terminal
  - For specific version use `sdk install java 21.0.6-tem`

2. **Verify Installation**
   - Open a terminal/command line.
   - Type `java --version`.
   - You should see `21.0.x` or similar.

### Package Installation

You can also install .NET 8 using a package manager:

- Chocolatey: Windows
- Homebrew: macOS

## Running the Java Application

To run the Java service using the Gradle wrapper, follow these steps:

- Navigate to the root directory of your Java service project in the terminal/command line. For this repository the command would be:
  - `cd java`

1. Run the following command to install the services dependencies: `gradle dependencies`
2. Run the following command to start up the application: `gradle run`

- The app should be available at: `http://localhost:4567`

3. (optional) Run the following command to execute the unit tests: `gradle test -i`

NOTE: if using the gradle wrapper (which doesn't require installing gradle, replace the above gradle commands with ./gradlew)

## Prompt Examples

Below are some prompts that can be tried from the `GamesController`. These prompts are purely for examples and do not product the final result of what is expected from the challenge.

### Generate a method for Loading words from API - Ghost Text

1. Create a method to Load words

```
  public List<String> LoadWordsFromApi() {
```

2. Hit return and review suggestions

### Naming Suggestion and edge cases - Inline Prompt - (control/cmd & i)

1. Highlight `getGame` method.
2. Explanation of the method.

```
/explain
```

3. Clarity on name.

```
Make this method name clear in intent
```

4. Handle edge cases.

```
This method should handle if a Game does not exist; it should return a NotFound response.
```

### Add Unit Test for makeGuess - Inline Prompt - (control/cmd & i)

1. Highlight MakeGuess
2. Suggest tests

```
/tests
```

### Building Guess Logic - Chat Prompt - (cmd + control/control + alt & i) - GamesController reference

1. Build out logic for makeGuess implementation.

```
MakeGuess needs to handle Checking a letter against a specific game determined by gameId. Update MakeGuess to review the letter against the game UnmaskedWord, update the game word property to unmask any correctly guessed letters and modify the RemainingGuesses account if an incorrect guess is made. If the RemainingGuesses count reaches, the Status of the game should move to "Lost" and no more guesses can be made
```

### Adding Validation - Chat Prompt - (cmd + control/control + alt & i) - GamesController reference

1. Implementation and usage

```
Validation is currently handled inline within the GamesController, specifically the makeGuess method. Design an approach to implement separated Validation including both the logic and the failure messages, this approach should be able to accommodate many validations happening within a validator and return all failure messages. This approach should have a validator within the Guess.java file, there should be a IValidatable inferface to return Validators, the Guess class should inherit from IValidatable, which will return the GuessValidator and lastly the GamesController.java should be updated to use the GuessValidator returned by the Guess.java model
```

### Adding Layered Architectural Pattern - Chat Prompt - (cmd + control/control + alt & i) - GamesController reference

1. Generates an example structure for a layered architecture.

```
Implement a layered architecture with a presentation layer, business layer and data access layer. Presentation handling the HttpRequests within Controllers for example GamesController, business for any logic for example GamesService and data access for managing the state of the game for example GamesRepository.
```
