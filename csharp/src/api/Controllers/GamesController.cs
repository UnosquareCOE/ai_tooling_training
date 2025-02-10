using System.Text.RegularExpressions;
using api.Constants;
using api.ViewModels;
using api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public partial class GamesController(IIdentifierGenerator identifierGenerator) : ControllerBase
{
    private static readonly Dictionary<Guid, GameViewModel> Games = new();
    private readonly string[] _words = ["banana", "canine", "unosquare", "airport"];

    [GeneratedRegex(@"[a-zA-Z0-9_]")]
    private static partial Regex GuessRegex();

    [HttpPost]
    public async Task<ActionResult<CreateGameResponseViewModel>> CreateGame([FromBody] CreateGameRequestViewModel request)
    {
        var newGameWord = await RetrieveWord(request.Language);
        var newGameId = identifierGenerator.RetrieveIdentifier();

        var newGame = new GameViewModel
        {
            RemainingGuesses = 5,
            UnmaskedWord = newGameWord,
            Word = GuessRegex().Replace(newGameWord, "_"),
            Status = GameStatuses.InProgress,
            IncorrectGuesses = []
        };

        Games.Add(newGameId, newGame);

        var response = new CreateGameResponseViewModel
        {
            GameId = newGameId,
            MaskedWord = newGame.Word,
            AttemptsRemaining = newGame.RemainingGuesses
        };

        return Ok(response);
    }

    [HttpGet("{gameId:guid}")]
    public ActionResult<GameViewModel> GetGame([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }
        var response = new MakeGuessResponseViewModel
        {
            MaskedWord = game.Word,
            AttemptsRemaining = game.RemainingGuesses,
            Guesses = game.IncorrectGuesses,
            Status = game.Status.ToString()
        };
        return Ok(response);
    }

    [HttpPut("{gameId:guid}")]
    public ActionResult<MakeGuessResponseViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "letter",
                        Message = "Letter cannot accept more than 1 character"
                    }
                ]
            });
        }
        
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }
        
        if (game.RemainingGuesses == 0 || game.Status != GameStatuses.InProgress)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The game is not in progress."
                    }
                ]
            });
        }
        
        var guessedLetter = guessViewModel.Letter.ToLower();
        ProcessGuess(game, guessedLetter);
        
        var response = new MakeGuessResponseViewModel
        {
            MaskedWord = game.Word,
            AttemptsRemaining = game.RemainingGuesses,
            Guesses = game.IncorrectGuesses,
            Status = game.Status.ToString()
        };
        
        return Ok(response);
    }
    
    [HttpGet("{gameId:guid}/cheat")]
    public ActionResult<string> Cheat([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }

        return Ok(game.UnmaskedWord);
    }
    
    [HttpDelete("{gameId:guid}")]
    public IActionResult DeleteGame([FromRoute] Guid gameId)
    {
        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Game not found",
                Errors =
                [
                    new ResponseErrorDetailViewModel
                    {
                        Field = "gameId",
                        Message = "The specified game ID does not exist."
                    }
                ]
            });
        }

        Games.Remove(gameId);
        return NoContent();
    }

    private static GameViewModel? RetrieveGame(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    private static async Task<string> RetrieveWord(string language)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync($"https://random-word-api.herokuapp.com/word?lang={language}&length=7");
        var words = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response);
        return words?.FirstOrDefault() ?? "defaultword";
    }
    
    private void ProcessGuess(GameViewModel game, string guessedLetter)
    {
        var unmaskedWord = game.UnmaskedWord!.ToLower();
        var maskedWord = game.Word!.ToCharArray();
        var correctGuess = false;

        for (var i = 0; i < unmaskedWord.Length; i++)
        {
            if (unmaskedWord[i] != guessedLetter[0]) continue;
            maskedWord[i] = unmaskedWord[i];
            correctGuess = true;
        }

        game.Word = new string(maskedWord);

        if (!correctGuess)
        {
            game.IncorrectGuesses.Add(guessedLetter);
            game.RemainingGuesses--;
        }

        if (game.Word == game.UnmaskedWord)
        {
            game.Status = GameStatuses.GameWon;
        }
        else if (game.RemainingGuesses == 0)
        {
            game.Status = GameStatuses.GameLost;
        }
    }
}