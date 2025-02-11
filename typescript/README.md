# Typescript Hangman AI Challenge

This project implements a Hangman game using the following technologies:

- Node version 22
- Node Package Manager (included with Node)
- [Express](https://expressjs.com/): A micro web framework used to expose API endpoints.
- [Jest](https://jestjs.io/): A Javascript testing framework focused on simplicity.

## How to Run the Application

Skip the installer sections if you already have the following installed:
- Node
- NPM

### Manual Installation on macOS & Windows

1. **Install Node 22**
- Visit the Node website at https://nodejs.org/en/download.
- Ensure the LTS (long-term support) tab is selected.
- Click on the installer relevant to your computer (pkg for macOS, msi for Windows).
- Follow the installation wizard with the default settings.

2. **Verify Installation**
- Open a terminal/command line.
- Type `node -v`.
  - You should see `v22.X.X`.

### Package Installation
You can also install Node 22 using a package manager:
- Chocolatey: Windows
- Homebrew: macOS
- [Linux Installation](https://nodejs.org/en/download/package-manager)

This approach can be more complex if issues arise and requires more terminal/command line experience. If using WSL on Windows, you can use Linux package managers depending on the distribution installed on WSL.

### Node Version Manager

Another approach for installing Node is using Node Version Manager (nvm). This is available for Linux or macOS. Follow the guide [here](https://github.com/nvm-sh/nvm).

Once installed, you can simply type:

```
nvm install 22
nvm use 22
```

## Running the Typescript Application

To run the Typescript service using Node & npm, follow these steps:

- Navigate to the root directory of your Typescript application in the terminal/command line. For this repository, the command would be:
  - `cd typescript`

1. Install the service dependencies: `npm install`
2. Start the application: `npm start`
  - The app should now be available at: `http://localhost:4567`
3. Execute the unit tests: `npm test`

## Prompt Examples
Below are some prompts that can be tried from the `games.ts` games controller. These prompts are purely for examples and do not product the final result of what is expected from the challenge.

### Generate a method for Loading words from API - Ghost Text
1. Create a method to Load words
  ```
    async function loadWordsFromApi() {
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
  Make it clear that GetGame gets a game based on an id
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

### Building Guess Logic - Chat Prompt - (cmd + control/control + alt & i)
1. Build out logic for MakeGuess implementation.
  ```
  makeGuess needs to handle Checking a letter against a specific game determined by gameId. Update makeGuess to review the letter against the game unmaskedWord, update the game word property to unmask any correctly guessed letters and modify the remainingGuesses account if an incorrect guess is made. If the remainingGuesses count reaches, the Status of the game should move to "Lost" and no more guesses can be made
  ```

### Adding express validator for Validation - Chat Prompt - (cmd + control/control + alt & i)
1. Implementation and usage for FluentValidation
  ```
  Validation is currently handled inline within the games.ts, specifically the makeGuess method. Express Validator can provide an approach to handle the existing validation for letter. Design an approach to implement Express Validator. This approach should use the game.ts within Routers for the validation configuration
  ```

### Adding Layered Architectural Pattern - Chat Prompt - (cmd + control/control + alt & i)
1. Generates an example structure for a layered architecture.
  ```
  Implement a layered architecture with a presentation layer, business layer and data access layer. Presentation handling the HttpRequests within Controllers and routers handling the request mapping, business for any logic for example GamesService and data access for managing the state of the game for example GamesRepository.
  ```
