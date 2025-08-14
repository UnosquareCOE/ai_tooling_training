# Evil AI Hangman Game - Sitemap

## URL Structure
Base URL: `https://project-evil-ai-hangman-game-667.magicpatterns.app/`

## Site Map

### 1. Home / Landing Page (`/`)
- **Initial State (Pre-game)**
  - Title: "HANGMAN"
  - Evil AI character with avatar
  - Welcome/Taunt message from Evil AI
  - Call-to-action button: "Accept your challenge" or "Come on, let's play"
  
### 2. Game State (`/` - Same URL, different states)

#### 2.1 Active Game State
- **Header Section**
  - Scoreboard:
    - Wins counter (top left)
    - "HANGMAN" title (center)
    - Losses counter (top right)
  
- **AI Character Section**
  - Evil AI avatar
  - Dynamic taunting messages based on game progress
  
- **Game Board**
  - Guesses remaining counter
  - Word display with underscores/revealed letters
  - Letter input field
  - "Guess" button (disabled until letter entered)
  - Guessed letters display section
  
#### 2.2 Game Over State - Loss
- Same layout as active game
- Final taunt message revealing the word
- Guesses left: 0
- Input disabled
- Refresh page to play again

#### 2.3 Game Over State - Win
- Same layout as active game  
- Victory acknowledgment message
- Complete word revealed
- Input disabled
- Refresh page to play again

## Game Flow

1. **Landing** → User clicks "Accept your challenge"/"Come on, let's play"
2. **Game Start** → Initialize with 5 guesses, empty word slots
3. **Gameplay Loop** → User enters letters, receives feedback
4. **Game End** → Win or Loss state with final message
5. **Reset** → Page refresh returns to landing state

## Interactive Elements

- **Buttons**
  - Start game button (landing page)
  - Guess button (during game)
  
- **Input Fields**  
  - Letter input textbox (single character)
  
- **Dynamic Content**
  - AI messages (changes based on game events)
  - Word display (reveals letters as guessed)
  - Guessed letters list (updates with each guess)
  - Counters (wins, losses, guesses remaining)

## Technical Notes

- Single Page Application (SPA)
- All game states handled on same URL path
- No additional pages or routes
- Game state managed client-side
- Refresh resets to initial landing state

## Mermaid Diagram

```mermaid
stateDiagram-v2
    [*] --> LandingPage: Load Site
    
    LandingPage --> ActiveGame: Click "Accept challenge"
    
    state ActiveGame {
        [*] --> GameInit: Start
        GameInit --> Playing: Initialize (5 guesses)
        
        Playing --> Playing: Guess Letter (Correct)
        Playing --> Playing: Guess Letter (Wrong)
        Playing --> GameWon: All Letters Found
        Playing --> GameLost: No Guesses Left
        
        GameWon --> [*]: Game Over
        GameLost --> [*]: Game Over
    }
    
    ActiveGame --> LandingPage: Page Refresh
    
    note right of LandingPage
        Components:
        - Title: "HANGMAN"
        - Evil AI Avatar
        - Taunt Message
        - Start Button
    end note
    
    note left of Playing
        Components:
        - Scoreboard (Wins/Losses)
        - AI Avatar & Messages
        - Word Display
        - Input Field
        - Guess Button
        - Guessed Letters List
        - Remaining Guesses
    end note
```

```mermaid
flowchart TD
    Start([Page Load]) --> Landing[Landing Page<br/>Evil AI Welcome]
    Landing -->|User clicks 'Accept'| Init[Initialize Game<br/>5 Guesses, Empty Word]
    
    Init --> Input[Enter Letter]
    Input --> Check{Letter in Word?}
    
    Check -->|Yes| Reveal[Reveal Letter Positions]
    Check -->|No| Decrement[Decrease Guesses]
    
    Reveal --> WordCheck{Word Complete?}
    WordCheck -->|Yes| Win[Game Won<br/>Victory Message]
    WordCheck -->|No| Input
    
    Decrement --> GuessCheck{Guesses > 0?}
    GuessCheck -->|Yes| Input
    GuessCheck -->|No| Lose[Game Lost<br/>Reveal Word]
    
    Win -->|Refresh| Start
    Lose -->|Refresh| Start
    
    style Landing fill:#f9f,stroke:#333,stroke-width:2px
    style Win fill:#9f9,stroke:#333,stroke-width:2px
    style Lose fill:#f99,stroke:#333,stroke-width:2px
```