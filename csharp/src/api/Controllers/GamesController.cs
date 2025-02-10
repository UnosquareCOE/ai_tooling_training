using System.Text.RegularExpressions;
using api.ViewModels;
using api.Utils;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.ViewModels;

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
    public ActionResult<CreateGameResponseViewModel> CreateGame()
    {
        var newGameWord = RetrieveWord();
        var newGameId = identifierGenerator.RetrieveIdentifier();

        var gameViewModel = new GameViewModel
        {
            RemainingGuesses = 5,
            UnmaskedWord = newGameWord,
            Word = GuessRegex().Replace(newGameWord, "_"),
            Status = "In Progress",
            IncorrectGuesses = new List<string>()
        };

        Games.Add(newGameId, gameViewModel);

        var response = new CreateGameResponseViewModel
        {
            GameId = newGameId,
            MaskedWord = gameViewModel.Word,
            AttemptsRemaining = gameViewModel.RemainingGuesses
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
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "gameId",
                        Message = "Game not found"
                    }
                }
            });
        }

        var checkGameViewModel = new CheckGameViewModel
        {
            MaskedWord = game.Word,
            AttemptsRemaining = game.RemainingGuesses,
            Status = game.Status,
            Guesses = game.IncorrectGuesses
        };

        return Ok(checkGameViewModel);
    }

    [HttpPut("{gameId:guid}")]
    public ActionResult<ProcessGameViewModel> MakeGuess([FromRoute] Guid gameId, [FromBody] GuessViewModel guessViewModel)
    {
        if (string.IsNullOrWhiteSpace(guessViewModel.Letter) || guessViewModel.Letter?.Length != 1)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "letter",
                        Message = "Letter cannot accept more than 1 character"
                    }
                }
            });
        }

        var game = RetrieveGame(gameId);
        if (game == null)
        {
            return NotFound(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "gameId",
                        Message = "Game not found"
                    }
                }
            });
        }

        if (game.RemainingGuesses == 0)
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "remainingGuesses",
                        Message = "No attempts remaining"
                    }
                }
            });
        }

        // Check if the letter has already been guessed
        if (game.IncorrectGuesses.Contains(guessViewModel.Letter))
        {
            return BadRequest(new ResponseErrorViewModel
            {
                Message = "Cannot process guess",
                Errors = new List<ErrorDetail>
                {
                    new ErrorDetail
                    {
                        Field = "letter",
                        Message = "Letter already guessed"
                    }
                }
            });
        }

        // if letter is in word, umask it from maskedWord
        var maskedWord = game.Word.ToCharArray();
        var unmaskedWord = game.UnmaskedWord.ToCharArray();
        var letter = guessViewModel.Letter[0];
        var letterFound = false;

        for (int i = 0; i < unmaskedWord.Length; i++)
        {
            if (unmaskedWord[i] == letter)
            {
                maskedWord[i] = letter;
                letterFound = true;
            }
        }

        if (!letterFound)
        {
            game.RemainingGuesses--;
            game.IncorrectGuesses.Add(letter.ToString());
        }

        game.Word = new string(maskedWord);

        var status = game.RemainingGuesses > 0 ? "In Progress" : "Game Over";
        if (!game.Word.Contains('_'))
        {
            status = "Won";
        }

        var processGameViewModel = new ProcessGameViewModel
        {
            MaskedWord = game.Word,
            AttemptsRemaining = game.RemainingGuesses,
            Guesses = game.IncorrectGuesses,
            Status = status
        };

        return Ok(processGameViewModel);
    }

    [HttpDelete("{gameId}")]
    public IActionResult ClearGame(Guid gameId)
    {
        if (Games.ContainsKey(gameId))
        {
            Games.Remove(gameId);
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }

    private static GameViewModel? RetrieveGame(Guid gameId)
    {
        return Games.GetValueOrDefault(gameId);
    }

    private string RetrieveWord()
    {
        return _words[new Random().Next(0, _words.Length)];
    }
}